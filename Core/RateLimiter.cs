using System;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace DrrrAsyncBot.Core
{
    public class RateLimiter
    {
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        private const int getRateLimit = 250;
        private static DateTime lastGet;
        private static bool getReady {
            get{
                if (lastGet == default)
                {
                    lastGet = DateTime.Now;
                    return true;
                }
                var diff = lastGet - DateTime.Now;
                if(diff.TotalMilliseconds > getRateLimit)
                    return true;
                return false;
            }
        }

        private const int postRateLimit = 350;
        private static DateTime lastPost;
        private static bool postReady {
            get{
                if (lastPost == default)
                {
                    lastPost = DateTime.Now;
                    return true;
                }
                var diff = lastPost - DateTime.Now;
                if(diff.TotalMilliseconds > postRateLimit)
                    return true;
                return false;
            }
        }

        public static async Task WaitGet () {
            if(!getReady)
                await Task.Delay(getRateLimit);
            await Lock.WaitAsync();
        }

        public static async Task WaitPost () {
            if(!postReady)
                await Task.Delay(postRateLimit);
            await Lock.WaitAsync();
        }

        public static void Release() {
            Lock.Release();
        }
    }
}