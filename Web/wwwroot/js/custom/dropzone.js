var drawingFileListData = [];
var dropArea = document.getElementById('drawingFileListDrop');
dropArea?.addEventListener('drop', handleDrop, false);

['dragenter', 'dragover', 'drop'].forEach(eventName => {
    dropArea?.addEventListener(eventName, preventDefaults, false)
    dropArea?.addEventListener(eventName, highlight1, false)
});

['dragleave', 'dragend', 'drop'].forEach(eventName => {
    dropArea?.addEventListener(eventName, unhighlight1, false)
});

$('#uploadDrawingBtn').off('change').on('change', function () {
    var drawingFiles = this.files;
    if (drawingFiles && drawingFiles.length > 0) {
        processAndAppendFiles(drawingFiles);
    }
});
$('.upload-img-container').off('click', '.zoom-side');
$('.upload-img-container').on('click', '.zoom-side', function (e) {
    e.stopPropagation(); // Prevent event bubbling
    var imageUrl = $(this).closest('.file-link').find('img').attr('src');
    window.open(imageUrl); // Open the image in a new window
});
// Remove file from file input on click
$('.upload-img-container').off('click', '.delete-side');
$('.upload-img-container').on('click', '.delete-side', function (e) {
    e.stopPropagation(); // Prevent event bubbling
    var _this = $(this);
    var wrap = _this.closest(".file-link");
    var reply = confirm("Are you sure you want to delete uploaded file? This action can not be undone.");
    var Id = _this.attr('data-file-id') || _this.data('file-id');
    if (Id == undefined) {
        wrap.remove();
        var files = $('#uploadDrawingBtn').prop('files');
        var updatedFiles = Array.from(files).filter(function (file) {
            return file.name !== _this.closest(".file-link").find(".uploaded-file-name").val();
        });

        // Create a new FileList object
        var newFileList = new DataTransfer();
        updatedFiles.forEach(function (file) {
            newFileList.items.add(file);
        });

        // Assign the new FileList to the file input
        $('#uploadDrawingBtn')[0].files = newFileList.files;

    }
    else {
        if (reply == true) {
            $.ajax({
                url: '/Asset/DeleteFile?Id=' + Id,
                type: "POST",
                success: function (data) {
                    var message = data.message;
                    wrap.remove();
                    alert(message);
                }
            });
        }
    }

    return false;
});


function preventDefaults(e) {
    e.preventDefault()
    e.stopPropagation()
}
function highlight1(e) {
    dropArea.classList.add('over')
}

function unhighlight1(e) {
    dropArea.classList.remove('over')
}
function handleDrop(e) {
    e.preventDefault();
    let dt = e.dataTransfer;
    let drawingFiles = dt.files;
    if (drawingFiles && drawingFiles.length > 0) {
        processAndAppendFiles(drawingFiles);
        $('#uploadDrawingBtn').prop('files', dt.files);
    }
}

function processAndAppendFiles(files) {

    var uploadImgContainer = $("#drawingFileList .upload-img-container");

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var reader = new FileReader();

        reader.onload = (function (file) {
            return function (e) {
                var imageUrl = e.target.result;
                var fileLinkDiv = $("<div>").addClass("file-link new mx-1").append(
                    $("<div>").addClass("actions").append(
                        $("<div>").attr("title", "Delete").addClass("delete-side").append(
                            $("<i>").addClass("fas fa-trash-alt")
                        ),
                        $("<div>").addClass("zoom-side").append(
                            $("<i>").addClass("fas fa-search-plus")
                        )
                    ),
                    $("<img>").attr({
                        src: imageUrl,
                        class: "img-thumbnail",
                        width: "100",
                        height: "100"
                    }),
                    $("<input>").attr({
                        type: "hidden",
                        class: "uploaded-file-name",
                        value: file.name
                    })
                );

                uploadImgContainer.append(fileLinkDiv);
            };
        })(file);

        reader.readAsDataURL(file);
    }
}