using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorized.HtmlTextEditor;

public static class Interop
{
    internal static async ValueTask<object> ConfigureStickyToolbar(
        IJSRuntime jsRuntime, ElementReference toolbarElement)
    {
        return await jsRuntime.InvokeAsync<object>(
            "window.QuillFunctions.configureStickyToolbar",
            toolbarElement);
    }

    static internal async ValueTask<bool> CreateQuill(
            IJSRuntime jsRuntime,
        ElementReference quillElement,
        ElementReference toolbar,
        bool readOnly,
        bool wrapImagesInFigures,
        string placeholder,
        string theme,
        string debugLevel,
        string scrollingContainerId,
        bool imageServerUploadEnabled,
        ImageServerUploadType imageServerUploadType,
        string imageServerUploadUrl,
        List<string>? customFonts = null)
    {
        return await jsRuntime.InvokeVoidAsyncWithErrorHandling(
             "window.QuillFunctions.createQuill",
             quillElement,
             toolbar,
             readOnly,
             wrapImagesInFigures,
             placeholder,
             theme,
             debugLevel,
             scrollingContainerId,
             imageServerUploadEnabled,
             imageServerUploadType.ToString(),
             imageServerUploadUrl,
             customFonts);
    }

    internal static async ValueTask<bool> EnableQuillEditor(IJSRuntime jsRuntime, ElementReference quillElement, bool mode)
    {
        return await jsRuntime.InvokeVoidAsyncWithErrorHandling("window.QuillFunctions.enableQuillEditor", quillElement, mode);
    }

    internal static async ValueTask<string> GetContent(
        IJSRuntime jsRuntime,
        ElementReference quillElement)
    {
        return await jsRuntime.InvokeAsync<string>("window.QuillFunctions.getQuillContent", quillElement);
    }

    internal static async ValueTask<string> GetHtml(
        IJSRuntime jsRuntime,
        ElementReference quillElement)
    {
        return await jsRuntime.InvokeAsync<string>("window.QuillFunctions.getQuillHTML", quillElement);
    }

    internal async static ValueTask<string> GetText(
                    IJSRuntime jsRuntime,
        ElementReference quillElement)
    {
        return await jsRuntime.InvokeAsync<string>("window.QuillFunctions.getQuillText", quillElement);
    }

    internal static async ValueTask<string> InsertQuillHtml(IJSRuntime jsRuntime, ElementReference quillElement, string html)
    {
        return await jsRuntime.InvokeAsync<string>("window.QuillFunctions.insertQuillHtml", quillElement, html);
    }

    internal static async ValueTask<string> InsertQuillImage(
            IJSRuntime jsRuntime,
        ElementReference quillElement,
        string imageUrl)
    {
        return await jsRuntime.InvokeAsync<string>("window.QuillFunctions.insertQuillImage", quillElement, imageUrl);
    }

    internal static async ValueTask<string> InsertQuillText(IJSRuntime jsRuntime, ElementReference quillElement, string text)
    {
        return await jsRuntime.InvokeAsync<string>("window.QuillFunctions.insertQuillText", quillElement, text);
    }

    internal static async ValueTask<string> LoadQuillContent(IJSRuntime jsRuntime, ElementReference quillElement, string content)
    {
        return await jsRuntime.InvokeAsync<string>("window.QuillFunctions.loadQuillContent", quillElement, content);
    }

    internal static async ValueTask<string> LoadQuillHtmlContent(IJSRuntime jsRuntime, ElementReference quillElement, string quillHtmlContent)
    {
        return await jsRuntime.InvokeAsync<string>("window.QuillFunctions.loadQuillHTMLContent", quillElement, quillHtmlContent);
    }

    internal static async ValueTask<bool> SetQuillBlazorBridge(IJSRuntime jsRuntime, ElementReference quillElement, DotNetObjectReference<HtmlTextEditor> objRef,
        string editorTextSaveUrl, string editorStatusElementId)
    {
        return await jsRuntime.InvokeVoidAsyncWithErrorHandling("window.QuillFunctions.setQuillBlazorBridge",
                            quillElement,
                            objRef,
                            editorTextSaveUrl,
                            editorStatusElementId);
    }
}