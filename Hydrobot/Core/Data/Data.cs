using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hydrobot.Resources.Database;
using System.Linq;

namespace Hydrobot.Core.Data {
    public static class Data {

        public static int GetOunces(ulong UserId) {
            using (var DbContext = new SqliteDbContext()) {
                if (DbContext.ounces.Where(x => x.UserId == UserId).Count() < 1) {
                    return 0;
                }
                return DbContext.ounces.Where(x => x.UserId == UserId).Select(x => x.Amount).FirstOrDefault();
            }
        }

        public static async Task SaveOunces(ulong UserId, int Amount) {
            using (var DbContext = new SqliteDbContext()) {
                if (DbContext.ounces.Where(x => x.UserId == UserId).Count() < 1) {
                    DbContext.ounces.Add(new ounce {
                        UserId = UserId,
                        Amount = Amount
                    });
                } else {
                    ounce Current = DbContext.ounces.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Amount += Amount;
                    DbContext.ounces.Update(Current);
                }
                await DbContext.SaveChangesAsync();
            }
        }
    }
}
