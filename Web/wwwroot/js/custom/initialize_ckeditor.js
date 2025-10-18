﻿//function initializeCkEditor(id) {
//    ClassicEditor
//        .create(document.getElementById(id), {
//            toolbar: {
//                items: [
//                    'exportPdf', 'exportWord', '|',
//                    'findAndReplace', 'selectAll', '|',
//                    'heading', '|',
//                    'bold', 'italic', 'strikethrough', 'underline', 'code', 'subscript', 'superscript', 'removeFormat', '|',
//                    'bulletedList', 'numberedList', 'todoList', '|',
//                    'outdent', 'indent', '|',
//                    'undo', 'redo',
//                    '-',
//                    'fontSize', 'fontFamily', 'fontColor', 'fontBackgroundColor', 'highlight', '|',
//                    'alignment', '|',
//                    'link', 'insertImage', 'blockQuote', 'insertTable', 'mediaEmbed', 'codeBlock', 'htmlEmbed', '|',
//                    'specialCharacters', 'horizontalLine', 'pageBreak', '|',
//                    'textPartLanguage', '|',
//                    'sourceEditing'
//                ],
//                shouldNotGroupWhenFull: true
//            },
//            list: {
//                properties: {
//                    styles: true,
//                    startIndex: true,
//                    reversed: true
//                }
//            },
//            heading: {
//                options: [
//                    { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
//                    { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
//                    { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' },
//                    { model: 'heading3', view: 'h3', title: 'Heading 3', class: 'ck-heading_heading3' },
//                    { model: 'heading4', view: 'h4', title: 'Heading 4', class: 'ck-heading_heading4' },
//                    { model: 'heading5', view: 'h5', title: 'Heading 5', class: 'ck-heading_heading5' },
//                    { model: 'heading6', view: 'h6', title: 'Heading 6', class: 'ck-heading_heading6' }
//                ]
//            },
//            placeholder: 'Welcome to CKEditor 5!',
//            fontFamily: {
//                options: [
//                    'default',
//                    'Arial, Helvetica, sans-serif',
//                    'Courier New, Courier, monospace',
//                    'Georgia, serif',
//                    'Lucida Sans Unicode, Lucida Grande, sans-serif',
//                    'Tahoma, Geneva, sans-serif',
//                    'Times New Roman, Times, serif',
//                    'Trebuchet MS, Helvetica, sans-serif',
//                    'Verdana, Geneva, sans-serif'
//                ],
//                supportAllValues: true
//            },
//            fontSize: {
//                options: [10, 12, 14, 'default', 18, 20, 22],
//                supportAllValues: true
//            },
//            htmlSupport: {
//                allow: [
//                    {
//                        name: /.*/,
//                        attributes: true,
//                        classes: true,
//                        styles: true
//                    }
//                ]
//            },
//            htmlEmbed: {
//                showPreviews: true
//            },
//            link: {
//                decorators: {
//                    addTargetToExternalLinks: true,
//                    defaultProtocol: 'https://',
//                    toggleDownloadable: {
//                        mode: 'manual',
//                        label: 'Downloadable',
//                        attributes: {
//                            download: 'file'
//                        }
//                    }
//                }
//            },
//            mention: {
//                feeds: [
//                    {
//                        marker: '@',
//                        feed: [
//                            'apple', 'bears', 'brownie', 'cake', 'candy', 'canes', 'chocolate', 'cookie', 'cotton', 'cream',
//                            'cupcake', 'danish', 'donut', 'dragée', 'fruitcake', 'gingerbread', 'gummi', 'ice', 'jelly-o',
//                            'liquorice', 'macaroon', 'marzipan', 'oat', 'pie', 'plum', 'pudding', 'sesame', 'snaps', 'soufflé',
//                            'sugar', 'sweet', 'topping', 'wafer'
//                        ],
//                        minimumCharacters: 1
//                    }
//                ]
//            },
//            removePlugins: [
//                'CKBox',
//                'CKFinder',
//                'EasyImage',
//                'RealTimeCollaborativeComments',
//                'RealTimeCollaborativeTrackChanges',
//                'RealTimeCollaborativeRevisionHistory',
//                'PresenceList',
//                'Comments',
//                'TrackChanges',
//                'TrackChangesData',
//                'RevisionHistory',
//                'Pagination',
//                'WProofreader',
//                'MathType',
//                'SlashCommand',
//                'Template',
//                'DocumentOutline',
//                'FormatPainter',
//                'TableOfContents'
//            ]
//        })
//        .then(editor => {
//            // Handle editor initialization...
//            // You might want to store the editor instance for further use.
//        })
//        .catch(error => {
//            console.error(error);
//        });
//}




function initializeCkEditor(id) {
    // This sample still does not showcase all CKEditor 5 features (!)
    CKEDITOR.ClassicEditor.create(document.getElementById(id), {
        toolbar: {
            items: [
                'exportPDF', 'exportWord', '|',
                'findAndReplace', 'selectAll', '|',
                'heading', '|',
                'bold', 'italic', 'strikethrough', 'underline', 'code', 'subscript', 'superscript', 'removeFormat', '|',
                'bulletedList', 'numberedList', 'todoList', '|',
                'outdent', 'indent', '|',
                'undo', 'redo',
                '-',
                'fontSize', 'fontFamily', 'fontColor', 'fontBackgroundColor', 'highlight', '|',
                'alignment', '|',
                'link', 'insertImage', 'blockQuote', 'insertTable', 'mediaEmbed', 'codeBlock', 'htmlEmbed', '|',
                'specialCharacters', 'horizontalLine', 'pageBreak', '|',
                'textPartLanguage', '|',
                'sourceEditing'
            ],
            shouldNotGroupWhenFull: true
        },
        // Changing the language of the interface requires loading the language file using the <script> tag.
        // language: 'es',
        list: {
            properties: {
                styles: true,
                startIndex: true,
                reversed: true
            }
        },
        heading: {
            options: [
                { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
                { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
                { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' },
                { model: 'heading3', view: 'h3', title: 'Heading 3', class: 'ck-heading_heading3' },
                { model: 'heading4', view: 'h4', title: 'Heading 4', class: 'ck-heading_heading4' },
                { model: 'heading5', view: 'h5', title: 'Heading 5', class: 'ck-heading_heading5' },
                { model: 'heading6', view: 'h6', title: 'Heading 6', class: 'ck-heading_heading6' }
            ]
        },
        placeholder: 'Welcome to CKEditor 5!',
        fontFamily: {
            options: [
                'default',
                'Arial, Helvetica, sans-serif',
                'Courier New, Courier, monospace',
                'Georgia, serif',
                'Lucida Sans Unicode, Lucida Grande, sans-serif',
                'Tahoma, Geneva, sans-serif',
                'Times New Roman, Times, serif',
                'Trebuchet MS, Helvetica, sans-serif',
                'Verdana, Geneva, sans-serif'
            ],
            supportAllValues: true
        },
        fontSize: {
            options: [10, 12, 14, 'default', 18, 20, 22],
            supportAllValues: true
        },
        // Be careful with the setting below. It instructs CKEditor to accept ALL HTML markup.
        htmlSupport: {
            allow: [
                {
                    name: /.*/,
                    attributes: true,
                    classes: true,
                    styles: true
                }
            ]
        },
        // Be careful with enabling previews
        htmlEmbed: {
            showPreviews: true
        },
        link: {
            decorators: {
                addTargetToExternalLinks: true,
                defaultProtocol: 'https://',
                toggleDownloadable: {
                    mode: 'manual',
                    label: 'Downloadable',
                    attributes: {
                        download: 'file'
                    }
                }
            }
        },
        mention: {
            feeds: [
                {
                    marker: '@',
                    feed: [
                        'apple', 'bears', 'brownie', 'cake', 'candy', 'canes', 'chocolate', 'cookie', 'cotton', 'cream',
                        'cupcake', 'danish', 'donut', 'dragée', 'fruitcake', 'gingerbread', 'gummi', 'ice', 'jelly-o',
                        'liquorice', 'macaroon', 'marzipan', 'oat', 'pie', 'plum', 'pudding', 'sesame', 'snaps', 'soufflé',
                        'sugar', 'sweet', 'topping', 'wafer'
                    ],
                    minimumCharacters: 1
                }
            ]
        },
        // The "super-build" contains more premium features that require additional configuration, disable them below.
        // Do not turn them on unless you read the documentation and know how to configure them and setup the editor.
        removePlugins: [
            // These two are commercial, but you can try them out without registering to a trial.
            // 'ExportPdf',
            // 'ExportWord',
            'CKBox',
            'CKFinder',
            'EasyImage',
            // This sample uses the Base64UploadAdapter to handle image uploads as it requires no configuration.
            // Storing images as Base64 is usually a very bad idea.
            // Replace it on production website with other solutions:
            // 'Base64UploadAdapter',
            'RealTimeCollaborativeComments',
            'RealTimeCollaborativeTrackChanges',
            'RealTimeCollaborativeRevisionHistory',
            'PresenceList',
            'Comments',
            'TrackChanges',
            'TrackChangesData',
            'RevisionHistory',
            'Pagination',
            'WProofreader',
            // Careful, with the Mathtype plugin CKEditor will not load when loading this sample
            'MathType',
            // The following features are part of the Productivity Pack and require additional license.
            'SlashCommand',
            'Template',
            'DocumentOutline',
            'FormatPainter',
            'TableOfContents'
        ]
    }).then(editor => {
        editors = {};
        editors[id] = editor;
    });
}