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
                try {
                    if (DbContext.ounces.Where(x => x.UserId == UserId).Count() < 1) {
                        return 0;
                    }
                    return DbContext.ounces.Where(x => x.UserId == UserId).Select(x => x.Amount).FirstOrDefault();
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return -1;
                }  
            }
        }

        public static async Task SaveOunces(ulong UserId, int Amount) {
            using (var DbContext = new SqliteDbContext()) {
                if (DbContext.ounces.Where(x => x.UserId == UserId).Count() < 1) {
                    DbContext.ounces.Add(new Ounce {
                        UserId = UserId,
                        Amount = Amount
                    });
                } else {
                    Ounce Current = DbContext.ounces.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Amount += Amount;
                    DbContext.ounces.Update(Current);
                }
                await DbContext.SaveChangesAsync();
            }
        }

        public static List<string> GetServerProcessNames() {
            using (var DbContext = new SqliteDbContext()) {
                try {
                    return DbContext.servers.Select(item => item.ProcessName).ToList();
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return new List<string>();
                }
            }
        }

        public static string GetServerExecutableLocation(string processName) {
            using (var DbContext = new SqliteDbContext()) {
                try {
                    return DbContext.servers.First(item => item.ProcessName.Contains(processName)).ExecutableLocation;
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return "";
                }
            }
        }

        public static string GetServerExecutableName(string processName) {
            using (var DbContext = new SqliteDbContext()) {
                try {
                    return DbContext.servers.First(item => item.ProcessName.Contains(processName)).ExecutableName;
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return "";
                }
            }
        }

        public static ulong GetServerPort(string processName) {
            using (var DbContext = new SqliteDbContext()) {
                try {
                    return DbContext.servers.First(item => item.ProcessName.Contains(processName)).Port;
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        public static string GetServerDescription(string processName) {
            using (var DbContext = new SqliteDbContext()) {
                try {
                    return DbContext.servers.First(item => item.ProcessName.Contains(processName)).Description;
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return "";
                }
            }
        }
    }
}
