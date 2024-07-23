using ThrowException.CSharpLibs.DatabaseObjectLib;
using NUnit.Framework;
using System;
using System.Linq;
using ThrowException.CSharpLibs.BytesUtilLib;
using ThrowException.CSharpLibs.PostgresDatabaseObjectLib;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.PostgresDatabaseObjectTest
{
    public class ParentObject : DatabaseObject
    {
        public StringField Name { get; private set; }
        public ReferenceListField<ChildObject, ParentObject> Children { get; private set; }
            = new ReferenceListField<ChildObject, ParentObject>(ChildObject.ParentFieldName, c => c.Parent);

        public ParentObject()
            : base()
        {
        }

        public ParentObject(Guid id, bool newObject = true)
            : base(id, newObject)
        {
        }
    }

    public class ChildObject : DatabaseObject
    {
        public const string ParentFieldName = "parentid";

        public StringField Name { get; private set; }

        [ColumnName(ParentFieldName)]
        public ReferenceDatabaseField<ParentObject, ChildObject> Parent { get; private set; }
            = new ReferenceDatabaseField<ParentObject, ChildObject>(p => p.Children);

        public ChildObject()
            : base()
        {
        }

        public ChildObject(Guid id, bool newObject = true)
            : base(id, newObject)
        {
        }
    }

    public class RefMigration : Migration
    {
        public RefMigration(IDatabase db)
            : base(db)
        { }

        protected override int Version => 1;

        protected override void Migrate(int version, ITransaction tx)
        {
            switch (version)
            {
                case 1:
                    Db.CreateTable<ParentObject>(tx);
                    Db.CreateTable<ChildObject>(tx);
                    break;
                default:
                    throw new ArgumentException("Invalid version");
            }
        }
    }

    [TestFixture()]
    public class RefTest
    {
        [Test()]
        public void References()
        {
            var db = new Database(ConnectionString.Get());

            using (var tx = db.BeginTransaction())
            {
                if (db.TableExists("childobject", tx))
                {
                    db.DropTable("childobject", tx);
                }

                if (db.TableExists("parentobject", tx))
                {
                    db.DropTable("parentobject", tx);
                }

                if (db.TableExists("meta", tx))
                {
                    db.DropTable("meta", tx);
                }

                tx.Commit();
            }

            var migration = new RefMigration(db);
            migration.Migrate();

            using (var tx = db.BeginTransaction())
            {
                var p = new ParentObject();
                p.Name.Value = "parent";
                db.Save(p, tx);

                var c0 = new ChildObject();
                c0.Name.Value = "child0";
                c0.Parent.Value = p;
                db.Save(c0, tx);

                var c1 = new ChildObject();
                c1.Name.Value = "child1";
                c1.Parent.Value = p;
                db.Save(c1, tx);

                var c2 = new ChildObject();
                c2.Name.Value = "child2";
                c2.Parent.Value = p;
                db.Save(c2, tx);

                tx.Commit();
            }

            using (var ctx = new DatabaseContext(db))
            {
                var childern = ctx.Load<ChildObject>();
                var c0 = childern.Single(c => c.Name.Value == "child0");
                var c1 = childern.Single(c => c.Name.Value == "child1");
                Assert.AreEqual("parent", c0.Parent.Value.Name.Value);
                Assert.AreEqual("parent", c1.Parent.Value.Name.Value);
                var parent = c0.Parent.Value;
                Assert.AreEqual(LoadingStatus.Partial, parent.Children.Status);
                Assert.AreEqual(2, parent.Children.Values.Count());
                Assert.IsTrue(parent.Children.Values.Contains(c0));
                Assert.IsTrue(parent.Children.Values.Contains(c1));
                parent.Children.Load();
                Assert.AreEqual(LoadingStatus.Fully, parent.Children.Status);
                Assert.AreEqual(3, parent.Children.Values.Count());
                var c2 = childern.Single(c => c.Name.Value == "child2");
                Assert.AreEqual("parent", c2.Parent.Value.Name.Value);
                Assert.IsTrue(parent.Children.Values.Contains(c2));
            }

            using (var ctx = new DatabaseContext(db))
            {
                Assert.AreEqual("child1", ctx.Load<ChildObject>(Condition.Equal("Name", "child1")).Single().Name.Value);
            }

            db.DropTable("childobject");
            db.DropTable("parentobject");
            db.DropTable("meta");
        }
    }
}
