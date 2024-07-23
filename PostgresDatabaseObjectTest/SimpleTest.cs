using ThrowException.CSharpLibs.DatabaseObjectLib;
using NUnit.Framework;
using System;
using System.Linq;
using ThrowException.CSharpLibs.BytesUtilLib;
using ThrowException.CSharpLibs.PostgresDatabaseObjectLib;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.PostgresDatabaseObjectTest
{
    public class SimpleObject : DatabaseObject
    {
        public ByteField Byte { get; private set; }
        public SByteField SByte { get; private set; }
        public Int16Field Int16 { get; private set; }
        public Int32Field Int32 { get; private set; }
        public Int64Field Int64 { get; private set; }
        public UInt16Field UInt16 { get; private set; }
        public UInt32Field UInt32 { get; private set; }
        public UInt64Field UInt64 { get; private set; }
        public FloatField Float { get; private set; }
        public DoubleField Double { get; private set; }
        public DecimalField Decimal { get; private set; }
        public GuidField Guid { get; private set; }
        public CharField Char { get; private set; }
        public StringField String { get; private set; }
        public BytesField Bytes { get; private set; }
        public BoolField Bool { get; private set; }
        public DateTimeField DateTime { get; private set; }
        public TimeSpanField TimeSpan { get; private set; }

        public SimpleObject()
            : base()
        {
        }

        public SimpleObject(Guid id, bool newObject = true)
            : base(id, newObject)
        {
        }
    }

    public class SimpleMigration : Migration
    {
        public SimpleMigration(IDatabase db)
            : base(db)
        { }

        protected override int Version => 1;

        protected override void Migrate(int version, ITransaction tx)
        {
            switch (version)
            {
                case 1:
                    Db.CreateTable<SimpleObject>(tx);
                    break;
                default:
                    throw new ArgumentException("Invalid version");
            }
        }
    }

    [TestFixture()]
    public class SimpleTest
    {
        [Test()]
        public void DataTypes()
        {
            var db = new Database(ConnectionString.Get());

            if (db.TableExists("simpleobject"))
            {
                db.DropTable("simpleobject");
            }

            if (db.TableExists("meta"))
            {
                db.DropTable("meta");
            }

            var migration = new SimpleMigration(db);
            migration.Migrate();

            db.Delete<SimpleObject>();

            var s1 = new SimpleObject();
            s1.Byte.Value = 1;
            s1.SByte.Value = -1;
            s1.Int16.Value = 30001;
            s1.UInt16.Value = 60001;
            s1.Int32.Value = 10000001;
            s1.UInt32.Value = uint.MaxValue - 1;
            s1.Int64.Value = long.MinValue + 1;
            s1.UInt64.Value = ulong.MaxValue - 1;
            s1.Char.Value = 'a';
            s1.String.Value = "alpha";
            s1.Bool.Value = true;
            s1.Guid.Value = Guid.Empty;
            s1.Bytes.Value = "a1a1".ParseHexBytes();
            s1.DateTime.Value = new DateTime(1960, 1, 1, 1, 1, 1);
            s1.TimeSpan.Value = new TimeSpan(10, 10, 10, 10);
            db.Save(s1);

            var s2 = new SimpleObject();
            s2.Byte.Value = 2;
            s2.SByte.Value = -2;
            s2.Int16.Value = 30002;
            s2.UInt16.Value = 60002;
            s2.Int32.Value = 10000002;
            s2.UInt32.Value = uint.MaxValue - 2;
            s2.Int64.Value = long.MinValue + 2;
            s2.UInt64.Value = ulong.MaxValue - 2;
            s2.Char.Value = 'b';
            s2.String.Value = "bravo";
            s2.Bool.Value = true;
            s2.Guid.Value = Guid.Empty;
            s2.Bytes.Value = "b2b2".ParseHexBytes();
            s2.DateTime.Value = new DateTime(2010, 1, 1, 1, 1, 1);
            s2.TimeSpan.Value = new TimeSpan(20, 10, 10, 10);
            db.Save(s2);

            var s3 = new SimpleObject();
            s3.Byte.Value = 3;
            s3.SByte.Value = -3;
            s3.Int16.Value = 30003;
            s3.UInt16.Value = 60003;
            s3.Int32.Value = 10000003;
            s3.UInt32.Value = uint.MaxValue - 3;
            s3.Int64.Value = long.MinValue + 3;
            s3.UInt64.Value = ulong.MaxValue - 3;
            s3.Char.Value = 'c';
            s3.String.Value = "charlie";
            s3.Bool.Value = true;
            s3.Guid.Value = Guid.Empty;
            s3.Bytes.Value = "c3c3".ParseHexBytes();
            s3.DateTime.Value = new DateTime(2024, 1, 1, 1, 1, 1);
            s3.TimeSpan.Value = new TimeSpan(600, 10, 10, 10);
            db.Save(s3);

            var l1 = db.Load<SimpleObject>().ToList();
            Assert.AreEqual(3, l1.Count);

            var o1 = l1.Single(o => o.Byte.Value == 1);
            Assert.AreEqual(1, o1.Byte.Value);
            Assert.AreEqual(-1, o1.SByte.Value);
            Assert.AreEqual(30001, o1.Int16.Value);
            Assert.AreEqual(60001, o1.UInt16.Value);
            Assert.AreEqual(10000001, o1.Int32.Value);
            Assert.AreEqual(uint.MaxValue - 1, o1.UInt32.Value);
            Assert.AreEqual(long.MinValue + 1, o1.Int64.Value);
            Assert.AreEqual(ulong.MaxValue - 1, o1.UInt64.Value);
            Assert.AreEqual('a', o1.Char.Value);
            Assert.AreEqual("alpha", o1.String.Value);
            Assert.AreEqual(true, o1.Bool.Value);
            Assert.AreEqual(Guid.Empty, o1.Guid.Value);
            Assert.IsTrue("a1a1".ParseHexBytes().AreEqual(o1.Bytes.Value));
            Assert.AreEqual(new DateTime(1960, 1, 1, 1, 1, 1), o1.DateTime.Value);
            Assert.AreEqual(new TimeSpan(10, 10, 10, 10), o1.TimeSpan.Value);

            var o2 = l1.Single(o => o.Byte.Value == 2);
            Assert.AreEqual(2, o2.Byte.Value);
            Assert.AreEqual(-2, o2.SByte.Value);
            Assert.AreEqual(30002, o2.Int16.Value);
            Assert.AreEqual(60002, o2.UInt16.Value);
            Assert.AreEqual(10000002, o2.Int32.Value);
            Assert.AreEqual(uint.MaxValue - 2, o2.UInt32.Value);
            Assert.AreEqual(long.MinValue + 2, o2.Int64.Value);
            Assert.AreEqual(ulong.MaxValue - 2, o2.UInt64.Value);
            Assert.AreEqual('b', o2.Char.Value);
            Assert.AreEqual("bravo", o2.String.Value);
            Assert.AreEqual(true, o2.Bool.Value);
            Assert.AreEqual(Guid.Empty, o2.Guid.Value);
            Assert.IsTrue("b2b2".ParseHexBytes().AreEqual(o2.Bytes.Value));
            Assert.AreEqual(new DateTime(2010, 1, 1, 1, 1, 1), o2.DateTime.Value);
            Assert.AreEqual(new TimeSpan(20, 10, 10, 10), o2.TimeSpan.Value);

            var o3 = l1.Single(o => o.Byte.Value == 3);
            Assert.AreEqual(3, o3.Byte.Value);
            Assert.AreEqual(-3, o3.SByte.Value);
            Assert.AreEqual(30003, o3.Int16.Value);
            Assert.AreEqual(60003, o3.UInt16.Value);
            Assert.AreEqual(10000003, o3.Int32.Value);
            Assert.AreEqual(uint.MaxValue - 3, o3.UInt32.Value);
            Assert.AreEqual(long.MinValue + 3, o3.Int64.Value);
            Assert.AreEqual(ulong.MaxValue - 3, o3.UInt64.Value);
            Assert.AreEqual('c', o3.Char.Value);
            Assert.AreEqual("charlie", o3.String.Value);
            Assert.AreEqual(true, o3.Bool.Value);
            Assert.AreEqual(Guid.Empty, o3.Guid.Value);
            Assert.IsTrue("c3c3".ParseHexBytes().AreEqual(o3.Bytes.Value));
            Assert.AreEqual(new DateTime(2024, 1, 1, 1, 1, 1), o3.DateTime.Value);
            Assert.AreEqual(new TimeSpan(600, 10, 10, 10), o3.TimeSpan.Value);

            Assert.AreEqual(3, db.Load<SimpleObject>(o3.Id).Byte.Value);
            Assert.AreEqual(3, db.Load<SimpleObject>(Condition.Equal("Byte", 3)).Single().Byte.Value);
            Assert.AreEqual(2, db.Load<SimpleObject>(Condition.Greater("Byte", 1).And(Condition.Smaller("Byte", 3))).Single().Byte.Value);
            Assert.AreEqual(2, db.Load<SimpleObject>(Condition.GreaterOrEqual("Byte", 2).And(Condition.SmallerOrEqual("Byte", 2))).Single().Byte.Value);
            Assert.AreEqual(2, db.Load<SimpleObject>(Condition.Greater("Byte", 2).Or(Condition.Smaller("Byte", 2))).Count());
            Assert.AreEqual(2, db.Load<SimpleObject>(Condition.NotEqual("Byte", 1)).Count());
            Assert.AreEqual(3, db.Load<SimpleObject>(Condition.Equal("String", "charlie")).Single().Byte.Value);

            db.DropTable("simpleobject");
            db.DropTable("meta");
        }
    }
}
