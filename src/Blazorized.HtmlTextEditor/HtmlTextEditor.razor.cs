using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static Blazorized.HtmlTextEditor.Interop;

namespace Blazorized.HtmlTextEditor;

public partial class HtmlTextEditor : ComponentBase, IDisposable, IAsyncDisposable
{
    private string? _interalValue = null;
    private MessageQueueProcessor _eventQueueProcessor = default!;

    private string _generatedToolBarId = ConstructToolbarId();

    private DotNetObjectReference<HtmlTextEditor>? _objRef;

    private ElementReference _quillElement;

    private ElementReference _toolBar;

    private bool _disposedValue;
    private bool _valueSetting = false;

    [Inject]
    public IJSRuntime? JsRuntime { get; set; }

    [Parameter]
    public string DebugLevel { get; set; } = "error";

    [Parameter]
    public int DelayInMsBetweenStatusChanges { get; set; } = 2000;

    [Parameter]
    public RenderFragment EditorContent { get; set; } = default!;

    [Parameter]
    public string EditorStatusElementId { get; set; } = default!;

    [Parameter]
    public List<string> Fonts { get; set; } = default!;

    [Parameter]
    public string Id { get; set; } = "ql-editor-container";

    [Parameter]
    public bool ImageServerUploadEnabled { get; set; } = false;

    [Parameter]
    public Func<string, string, Stream, Task<string>>? ImageServerUploadMethod { get; set; }

    [Parameter]
    public ImageServerUploadType ImageServerUploadType { get; set; } = ImageServerUploadType.ApiPost;

    [Parameter]
    public string ImageServerUploadUrl { get; set; } = default!;

    [Parameter]
    public EventCallback<string> BeforeValueChanged { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public EventCallback ValueSaved { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "Compose an epic...";

    [Parameter]
    public bool ReadOnly { get; set; } = false;

    [Parameter]
    public bool StickyToolBar { get; set; } = false;

    [Parameter]
    public string TextSavePostUrl { get; set; } = default!;

    [Parameter]
    public EditorTheme Theme { get; set; } = EditorTheme.Snow;

    [Parameter]
    public Toolbar Toolbar { get; set; } = default!;

    [Parameter]
    public RenderFragment ToolbarContent { get; set; } = default!;

    [Parameter]
    public bool WrapImagesInFigures { get; set; } = true;

    public HtmlTextEditor()
    {

    }

    public HtmlTextEditor(IJSRuntime? jsRuntime) : this()
    {
        if (jsRuntime != null)
            this.JsRuntime = jsRuntime;
    }

    public async Task BundleAndSetQuillBlazorBridge()
    {
        _objRef = DotNetObjectReference.Create(this);
        if (JsRuntime != null)
        {
            //Wire up our bridge between quill and blazor in the javascript
            await Interop.SetQuillBlazorBridge(
                JsRuntime,
                _quillElement,
                _objRef,
                TextSavePostUrl,
                EditorStatusElementId);
        }
    }

    [Parameter]
    public string Value
    {
        get
        {
            return _interalValue ?? string.Empty;
        }
        set
        {
            SetValue(value).AndForget();
        }
    }

    private async Task SetValue(string value, bool internalSet = true)
    {
        if (_interalValue != value && !_valueSetting)
        {
            try
            {
                _valueSetting = true;
                if (internalSet)
                    _interalValue = await LoadHtmlContent(value);

                await ValueChanged.InvokeAsync(_interalValue);
            }
            finally
            {
                _valueSetting = false;
            }
        }
    }

    public async Task EnableEditor(bool mode)
    {
        await Interop.EnableQuillEditor(JsRuntime!, _quillElement, mode);
    }

    [JSInvokable]
    public void EnqueueStatusMessage(string statusMessage)
    {
        _eventQueueProcessor.Enqueue(statusMessage);
    }

    [JSInvokable]
    public async Task FireTextChangedEvent(string html)
    {
        _interalValue = html;
        await ValueChanged.InvokeAsync(_interalValue);
    }

    [JSInvokable]
    public async Task FireTextSavedEvent()
    {
        await ValueSaved.InvokeAsync();
    }

    public async ValueTask<string> GetContent()
    {
        return await Interop.GetContent(JsRuntime!, _quillElement);
    }

    public async ValueTask<string> GetHtml()
    {
        _interalValue = await Interop.GetHtml(JsRuntime!, _quillElement);
        return _interalValue;
    }

    public async ValueTask<string> GetText()
    {
        return await Interop.GetText(JsRuntime!, _quillElement);
    }

    public async Task InsertImage(string imageUrl)
    {
        var value = await Interop.InsertQuillImage(JsRuntime!, _quillElement, imageUrl);
        await SetValue(value, false);
    }

    public async ValueTask<string> InsertText(string text)
    {
        //Insert as html to keep any current html formating
        _interalValue = await Interop.InsertQuillHtml(JsRuntime!, _quillElement, text);
        return _interalValue;
    }

    public async ValueTask<string> InsertHtml(string html)
    {
        _interalValue = await Interop.InsertQuillHtml(JsRuntime!, _quillElement, html);
        return _interalValue;
    }

    public async ValueTask<string> LoadContent(string content)
    {
        _interalValue = await Interop.LoadQuillContent(JsRuntime!, _quillElement, content);
        return _interalValue;
    }

    public async ValueTask<string> LoadHtmlContent(string quillHtmlContent)
    {
        _interalValue = await Interop.LoadQuillHtmlContent(JsRuntime!, _quillElement, quillHtmlContent);
        return _interalValue;
    }

    [JSInvokable]
    public async Task<string> SaveImage(string imageName, string fileType)
    {
        var dataReference = await JsRuntime!.InvokeAsync<IJSStreamReference>("quillImageDataStream");
        await using var dataReferenceStream = await dataReference.OpenReadStreamAsync(maxAllowedSize: 10_000_000);

        return ImageServerUploadMethod == null ? ""
            : await ImageServerUploadMethod(imageName, fileType, dataReferenceStream);
    }

    public async Task SetTextPostUrlAsync(string textSavePostUrl)
    {
        TextSavePostUrl = textSavePostUrl;
        await JsRuntime!.InvokeVoidAsync("window.QuillFunctions.setTextSavePostUrl", textSavePostUrl);
    }

    public async Task ShowStatusMessage(string message)
    {
        await JsRuntime!.InvokeVoidAsync("window.QuillFunctions.ShowStatusMessage", message);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Interop.CreateQuill(
                      JsRuntime!,
                      _quillElement,
                      _toolBar,
                      ReadOnly,
                      WrapImagesInFigures,
                      Placeholder,
                      Theme.ToString().ToLower(),
                      DebugLevel,
                      Id,
                      ImageServerUploadEnabled,
                      ImageServerUploadType,
                      ImageServerUploadUrl,
                      Fonts);

            await ScrollEventHandler();
            await BundleAndSetQuillBlazorBridge();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _eventQueueProcessor = new MessageQueueProcessor(this);
        if (ImageServerUploadEnabled)
        {
            switch (ImageServerUploadType)
            {
                case ImageServerUploadType.ApiPost:
                    if (string.IsNullOrEmpty(ImageServerUploadUrl))
                    {
                        throw new ArgumentNullException($"You cannot set the ImageServerUploadEnabled with ImageServerUploadType of ImageServerUploadType.ApiPost without providing the ImageServerUploadUrl parameter.");
                    }
                    break;

                case ImageServerUploadType.BlazorMethod:
                    if (ImageServerUploadMethod == null)
                    {
                        throw new ArgumentNullException($"You cannot set the ImageServerUploadEnabled with ImageServerUploadType of ImageServerUploadType.BlazorMethod without providing an implementation of the ImageServerUploadMethod Function");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static string ConstructToolbarId()
    {
        var random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var rando = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return $"toolbar-{rando}";
    }

    private string CalculateToolBarClass()
    {
        var retVal = "toolbar";

        if (Theme.Equals(EditorTheme.Bubble))
        {
            return "";
        }

        if (StickyToolBar)
        {
            retVal += " sticky";
        }

        return retVal;
    }

    private async Task ScrollEventHandler()
    {
        if (StickyToolBar)
        {
            await Interop.ConfigureStickyToolbar(JsRuntime!, _toolBar);
        }
    }

    #region IDisposable Pattern

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposedValue)
        {
            if (JsRuntime != null)
            {
                JsRuntime.InvokeAsync<string>("window.QuillFunctions.unBindToQuillTextChangeEvent").ConfigureAwait(false);
            }
        }

        if (!_disposedValue)
        {
            _objRef?.Dispose();
            _objRef = null;
        }

        _disposedValue = true;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (JsRuntime != null)
        {
            await JsRuntime.InvokeAsync<string>("window.QuillFunctions.unBindToQuillTextChangeEvent");
        }

        Dispose(disposing: false);
    }

    #endregion IDisposable Pattern
}