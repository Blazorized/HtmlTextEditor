
# Blazored TextEditor
Html Text Editor for Blazor applications - Uses [Quill JS](https://quilljs.com/ "Quill JS.com")
This project merges the best parts of [Blazored.TextEditor](https://github.com/Blazored/TextEditor "Blazored.TextEditor") and [WYSIWYGTextEditor](https://github.com/somegenericdev/WYSIWYGTextEditor "WYSIWYGTextEditor ") into one version.

![Screenshot](screenshot.png)

### Sample Applications

* [Simple Blazor WASM application](https://github.com/Blazorized/HtmlTextEditor/tree/main/samples/BlazorWasmApp "BlazorWasmApp") - [Contains an example of uploading images and inserting html]

### Helpful Articles

* [Creating A Rich Text Editor In Blazor Using Quill](https://blazorhelpwebsite.com/ViewBlogPost/12 "BlazorHelpWebsite.com")

### Installing

You can install from NuGet using the following command:

`Install-Package Blazorized.HtmlTextEditor`

Or via the Visual Studio package manger.

### Setup
Blazor Server applications will need to include the following CSS and JS files in their `Pages\_Host.cshtml` (or `Pages/_Layout.cshtml` if using .Net 6).

In the `head` tag add the following CSS.

```html
    <link href="_content/Blazorized.HtmlTextEditor/blazor-quill.css" rel="stylesheet">
    <link href="//cdn.quilljs.com/1.3.6/quill.snow.css" rel="stylesheet">
    <link href="//cdn.quilljs.com/1.3.6/quill.bubble.css" rel="stylesheet">
```

Then add the JS script at the bottom of the page using the following script tag.

```html
    <script src="https://cdn.quilljs.com/1.3.6/quill.js"></script>
    <script src="_content/Blazorized.HtmlTextEditor/quill-blot-formatter.js"></script>
    <script src="_content/Blazorized.HtmlTextEditor/blazor-quill.js"></script>
```

**NOTE** If you're using Blazor WebAssembly then these need to be added to your `wwwroot\index.html`.

You can add the following using statement to your main `_Imports.razor` to make referencing the component a bit easier.

```cs
@using Blazorized.HtmlTextEditor
```

## Usage

Below is a list of all the options available on the Text Editor.

**Templates**

- `ToolbarContent` (optional) - Allows the user to define the Toolbar (above the editor control, or in-line when using the bubble theme, and a user highlights text in the editor).
- `EditorContent` (optional) - Allows the user to define the initial content

**Parameters**

- `ReadOnly` (Optional - Default: `false`) - Determines if the editor will load in read-only mode. This mode can be toggled.
- `Placeholder` (Optional - Default: `Compose an epic...`) - The text to show when editor is empty.
- `Theme` (Optional - Default: `snow`) - Use `snow` to show the Toolbar on top of the editor, and `bubble` for inline editing.
- `DebugLevel` (Optional - Default: `info`) - Determines the level of debug information returned to the web browser console window. Options are `error`, `warn`, `log`, or `info`.

**Methods**

- `GetText` - Gets the content of the editor as Text.
- `GetHtml` - Gets the content of the editor as HTML.
- `GetContent` - Gets the content of the editor in the native Quill JSON Delta format.
- `LoadContent` (`json`) - Allows the content of the editor to be programmatically set.
- `LoadHtmlContent` (`string`) - Allows the content of the editor to be programmatically set.
- `InsertHtml` (`string`) - Inserts HTML at the current point in the editor.
- `InsertImage` (`string`) - Inserts an image at the current point in the editor.
- `InsertText` (`string`) - Inserts text at the current point in the editor.


### Basic Example
(see code in the [Index.razor page](https://github.com/Blazored/TextEditor/blob/main/samples/BlazorServerSide/Pages/Index.razor) in the repo for more examples)
```cs
@using Blazorized.HtmlTextEditor

<HtmlTextEditor @ref="@QuillHtml">
    <ToolbarContent>
        <select class="ql-header">
            <option selected=""></option>
            <option value="1"></option>
            <option value="2"></option>
            <option value="3"></option>
            <option value="4"></option>
            <option value="5"></option>
        </select>
        <span class="ql-formats">
            <button class="ql-bold"></button>
            <button class="ql-italic"></button>
            <button class="ql-underline"></button>
            <button class="ql-strike"></button>
        </span>
        <span class="ql-formats">
            <select class="ql-color"></select>
            <select class="ql-background"></select>
        </span>
        <span class="ql-formats">
            <button class="ql-list" value="ordered"></button>
            <button class="ql-list" value="bullet"></button>
        </span>
        <span class="ql-formats">
            <button class="ql-link"></button>
        </span>
    </ToolbarContent>
    <EditorContent>
        <h4>This Toolbar works with HTML</h4>
        <a href="http://BlazorHelpWebsite.com">
        BlazorHelpWebsite.com</a>
    </EditorContent>
</HtmlTextEditor>
<br />
<button class="btn btn-primary" 
        @onclick="GetHtml">Get HTML</button>
<button class="btn btn-primary"
        @onclick="SetHTML">Set HTML</button>
<br />
<div>
    <br />
    @((MarkupString)QuillHTMLContent)
    @QuillHTMLContent
</div>
<br />

@code {

HtmlTextEditor QuillHtml;
string QuillHTMLContent;

    public async void GetHtml()
    {
        QuillHTMLContent = await this.QuillHtml.GetHtml();
        StateHasChanged();
    }

    public async void SetHTML()
    {
        string QuillContent =
            @"<a href='http://BlazorHelpWebsite.com/'>" +
            "<img src='images/BlazorHelpWebsite.gif' /></a>";

        await this.QuillHtml.LoadHtmlContent(QuillContent);
        StateHasChanged();
    }
}
```