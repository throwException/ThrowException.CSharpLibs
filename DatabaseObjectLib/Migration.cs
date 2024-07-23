using System;
using System.Linq;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public abstract class Migration
    {
        public IDatabase Db { get; private set; }

        protected Migration(IDatabase db)
        {
            Db = db;
        }

        public void Migrate()
        {
            using (var tx = Db.BeginTransaction())
            {
                if (!Db.TableExists("meta", tx))
                {
                    Db.CreateTable<Meta>(tx);
                    var meta = new Meta();
                    meta.Version.Value = 0;
                    Db.Save(meta, tx);
                    tx.Commit();
                }
            }

            while (true)
            {
                using (var tx = Db.BeginTransaction())
                {
                    var meta = Db.Load<Meta>().Single();
                    if (meta.Version.Value < Version)
                    {
                        meta.Version.Value++;
                        Migrate(meta.Version.Value, tx);
                        Db.Save(meta, tx);
                        tx.Commit();
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        protected abstract int Version { get; }

        protected abstract void Migrate(int version, ITransaction tx);
    }
}
