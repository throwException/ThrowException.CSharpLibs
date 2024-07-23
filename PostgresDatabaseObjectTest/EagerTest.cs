using ThrowException.CSharpLibs.DatabaseObjectLib;
using NUnit.Framework;
using System;
using System.Linq;
using ThrowException.CSharpLibs.BytesUtilLib;
using ThrowException.CSharpLibs.PostgresDatabaseObjectLib;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.PostgresDatabaseObjectTest
{
    public class EagerParent : DatabaseObject
    {
        public StringField Name { get; private set; }

        [Loading(LoadingBehavior.Eager)]
        public ReferenceListField<EagerChild, EagerParent> Children { get; private set; }
            = new ReferenceListField<EagerChild, EagerParent>(ChildObject.ParentFieldName, c => c.Parent);

        public EagerParent()
            : base()
        {
        }

        public EagerParent(Guid id, bool newObject = true)
            : base(id, newObject)
        {
        }
    }

    public class EagerChild : DatabaseObject
    {
        public const string ParentFieldName = "parentid";

        public StringField Name { get; private set; }

        [Loading(LoadingBehavior.Eager)]
        [ColumnName(ParentFieldName)]
        public ReferenceDatabaseField<EagerParent, EagerChild> Parent { get; private set; }
            = new ReferenceDatabaseField<EagerParent, EagerChild>(p => p.Children);

        public EagerChild()
            : base()
        {
        }

        public EagerChild(Guid id, bool newObject = true)
            : base(id, newObject)
        {
        }
    }

    public class EagerMigration : Migration
    {
        public EagerMigration(IDatabase db)
            : base(db)
        { }

        protected override int Version => 1;

        protected override void Migrate(int version, ITransaction tx)
        {
            switch (version)
            {
                case 1:
                    Db.CreateTable<EagerParent>(tx);
                    Db.CreateTable<EagerChild>(tx);
                    break;
                default:
                    throw new ArgumentException("Invalid version");
            }
        }
    }

    [TestFixture()]
    public class EagerTest
    {
        [Test()]
        public void References()
        {
            var db = new Database(ConnectionString.Get());

            using (var tx = db.BeginTransaction())
            {
                if (db.TableExists("eagerchild", tx))
                {
                    db.DropTable("eagerchild", tx);
                }

                if (db.TableExists("eagerparent", tx))
                {
                    db.DropTable("eagerparent", tx);
                }

                if (db.TableExists("meta", tx))
                {
                    db.DropTable("meta", tx);
                }

                tx.Commit();
            }

            var migration = new EagerMigration(db);
            migration.Migrate();

            using (var tx = db.BeginTransaction())
            {
                var p = new EagerParent();
                p.Name.Value = "parent";
                db.Save(p, tx);

                var c0 = new EagerChild();
                c0.Name.Value = "child0";
                c0.Parent.Value = p;
                db.Save(c0, tx);

                var c1 = new EagerChild();
                c1.Name.Value = "child1";
                c1.Parent.Value = p;
                db.Save(c1, tx);

                var c2 = new EagerChild();
                c2.Name.Value = "child2";
                c2.Parent.Value = p;
                db.Save(c2, tx);

                tx.Commit();
            }

            using (var ctx = new DatabaseContext(db))
            {
                var childern = ctx.Load<EagerChild>().ToList();
                var c0 = childern.Single(c => c.Name.Value == "child0");
                var c1 = childern.Single(c => c.Name.Value == "child1");
                Assert.AreEqual("parent", c0.Parent.Value.Name.Value);
                Assert.AreEqual("parent", c1.Parent.Value.Name.Value);
                var parent = c0.Parent.Value;
                Assert.AreEqual(LoadingStatus.Fully, parent.Children.Status);
                Assert.AreEqual(3, parent.Children.Values.Count());
                Assert.IsTrue(parent.Children.Values.Contains(c0));
                Assert.IsTrue(parent.Children.Values.Contains(c1));
                var c2 = childern.Single(c => c.Name.Value == "child2");
                Assert.IsTrue(parent.Children.Values.Contains(c2));
            }

            using (var ctx = new DatabaseContext(db))
            {
                Assert.AreEqual("child1", ctx.Load<EagerChild>(Condition.Equal("Name", "child1")).Single().Name.Value);
            }

            db.DropTable("eagerchild");
            db.DropTable("eagerparent");
            db.DropTable("meta");
        }
    }
}
