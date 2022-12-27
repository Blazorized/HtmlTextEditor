using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorized.HtmlTextEditor
{
    internal static class _InternalExtension
    {
        public static async ValueTask<bool> InvokeVoidAsyncWithErrorHandling(this IJSRuntime jsRuntime, string identifier, params object[] args)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync(identifier, args);
                return true;
            }
            catch (JSException)
            {
                return false;
            }
            catch (JSDisconnectedException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }


        public static async void AndForget(this Task task)
        {
            await task; // async void -->  awful but open to ideas
        }
    }
}
