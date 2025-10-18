
function SwalErrorAlert(message) {
    swal.fire({
        title: 'Error!',
        text: message,
        icon: 'error',
        showCancelButton: false,
        confirmButtonText: "Okay",
        cancelButtonText: "Cancel",
        confirmButtonClass: 'btn btn-success m-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    });
} 

function SwalSuccessAlert(message) {
    swal.fire({
        title: 'Success!',
        text: message,
        icon: 'success',
        showCancelButton: false,
        confirmButtonText: "Okay",
        cancelButtonText: "Cancel",
        confirmButtonClass: 'btn btn-success m-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    });
} 
