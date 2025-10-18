$(function () {


    $("#statusSelect").val('');
    $("#severitySelect").val('');

    GetIncidentList(0, 0, "");

    $(document).off("click", "#nextToIncidentLocation");
    $(document).on("click", "#nextToIncidentLocation", function (e) {
        e.preventDefault();

        var isValid = true;

        // Loop through all required fields
        $("#pills-caller").find("input[data-val-required], select[data-val-required], textarea[data-val-required], #relationaship, #CallTimedatetime").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            // Dropdown special check
            if ($field.is("select") && (value === "" || value === "--Select--")) {
                isValid = false;
                showError($field);
            }
            else if (value === "") {
                isValid = false;
                showError($field);
            }
            else {
                clearError($field);
            }
        });

        if (isValid) {
            $("#pills-location-tab").trigger("click");
        }
    });

    $(document).off("click", "#nextToAdditionalLocations");
    $(document).on("click", "#nextToAdditionalLocations", function (e) {
        e.preventDefault();

        var isValid = true;

        // Loop through all required fields, excluding the serviceAct field
        $("#pills-location").find("input[data-val-required]:not(#serviceAct), textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            // Dropdown special check
            if ($field.is("select") && (value === "" || value === "--Select--")) {
                isValid = false;
                showError($field);
            }
            else if (value === "") {
                isValid = false;
                showError($field);
            }
            else {
                clearError($field);
            }
        });

        if (isValid) {
            $("#pills-additional-tab").trigger("click");
        }
    });

    $(document).off("click", "#nextToIncidentDetials");
    $(document).on("click", "#nextToIncidentDetials", function (e) {
        e.preventDefault();
        var isValid = true;
        if ($("#locationForm").is(":visible")) {
            if ($("#addlocAddress").val() === "") {
                showError($("#addlocAddress"));
                isValid = false;
            }
        }

        if (isValid) {
            $("#pills-detail-tab").trigger("click");
        }
    });


    $(document).off("click", "#nextToDescriptionIssue");
    $(document).on("click", "#nextToDescriptionIssue", function (e) {
        e.preventDefault();

        var isValid = true;

        if (!$("#IsOtherEvent").is(":checked")) {
            if ($(".eventCheck input[type='checkbox']:checked").length === 0) {
                isValid = false;
                $(".eventCheck input[type='checkbox']").css("outline", "2px solid red");
            } else {
                $(".eventCheck input[type='checkbox']").css("outline", "none");
            }
        }
        else {
            $(".eventCheck input[type='checkbox']").css("outline", "none");
            var OtherEventDetailText = $("#OtherEventDetail").val();
            if (OtherEventDetailText == "") {
                isValid = false;
                showError($("#OtherEventDetail"));
            }
            else {
                clearError($("#OtherEventDetail"));
            }
        }

        if (isValid) {
            $("#pills-description-tab").trigger("click");
        }
    });

    $(document).off("click", "#nextToSeverity");
    $(document).on("click", "#nextToSeverity", function (e) {
        e.preventDefault();

        var isValid = true;

        // Loop through all required fields
        $("#pills-description").find("textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            // Dropdown special check
            if ($field.is("select") && (value === "" || value === "--Select--")) {
                isValid = false;
                showError($field);
            }
            else if (value === "") {
                isValid = false;
                showError($field);
            }
            else {
                clearError($field);
            }
        });

        if (isValid) {
            $("#pills-severity-tab").trigger("click");
        }
    });

    $(document).off("click", "#nextToEnvironmental");
    $(document).on("click", "#nextToEnvironmental", function (e) {
        e.preventDefault();

        var isValid = true;

        // Loop through all required fields
        //$("#pills-severity").find("#severity").each(function () {
        //    var $field = $(this);
        //    var value = $.trim($field.val());

        //    // Dropdown special check
        //    if ($field.is("select") && (value === "" || value === "--Select--")) {
        //        isValid = false;
        //        showError($field);
        //    }
        //    else if (value === "") {
        //        isValid = false;
        //        showError($field);
        //    }
        //    else {
        //        clearError($field);
        //    }
        //});

        // Loop through all required fields
        $("#pills-description").find("textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            if (value === "") {
                isValid = false;
                showError($field);
            }
            else {
                clearError($field);
            }
        });

        if (isValid) {
            $("#pills-safety-tab").trigger("click");
        }
    });

    $(document).off("click", "#nextToSupportInfo");
    $(document).on("click", "#nextToSupportInfo", function (e) {
        e.preventDefault();

        var isValid = true;

        // Loop through all required fields
        $("#pills-safety").find("input[data-val-required], select[data-val-required], textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            // Dropdown special check
            if ($field.is("select") && (value === "" || value === "--Select--")) {
                isValid = false;
                showError($field);
            }
            else if (value === "") {
                isValid = false;
                showError($field);
            }
            else {
                clearError($field);
            }
        });

        if (isValid) {
            $("#pills-support-tab").trigger("click");
        }
    });

    $(document).on("click", "#btn_Incident_Save", function (e) {
        e.preventDefault();

        var isValid = true;

        // Loop through all required fields
        $("#pills-support").find("select[data-val-required], textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            // Dropdown special check
            if ($field.is("select") && (value === "" || value === "--Select--")) {
                isValid = false;
                showError($field);
            }
            else if (value === "") {
                isValid = false;
                showError($field);
            }
            else {
                clearError($field);
            }
        });

        if (isValid) {
            SaveIncidentForm();
        }
    });

    //$(document).off("click", ".statusLegendli");
    //$(document).on("click", ".statusLegendli", function (e) {
    //    e.preventDefault();

    //    $("#statusSelect").val('');
    //    $("#severitySelect").val('');

    //    var incidentID = $(this).closest('tr').attr('id');
    //    var status = $(this).find('div.dropdown-item').attr('data-id');

    //    status = $.trim(status);

    //    ChangeIncidentStatus(incidentID, status);
    //});

    $(document).off("change", "#statusSelect, #severitySelect");
    $(document).on("change", "#statusSelect, #severitySelect", function (e) {

        var statusID = $("#statusSelect").val() != "" ? $("#statusSelect").val() : 0;
        var severityID = $("#severitySelect").val() != "" ? $("#severitySelect").val() : 0;
        var globalSearch = $("#global_search_value").val() != "" ? $("#global_search_value").val() : "";

        e.preventDefault();
        GetIncidentList(statusID, severityID, globalSearch);
    });

    $(document).off("change", "#isSameCallerAddress");
    $(document).on("change", "#isSameCallerAddress", function (e) {
        if ($(this).is(":checked")) {
            $("#locAddress").val($("#callerAddress").val());

            $.getJSON("/Incidents/Resolve", { magicKey: $("#hdnmagicKey").val() }, function (data) {
                $("#Loc1Latitude").val(data.lat);
                $("#Loca1Longitude").val(data.lon);
            });
        }
        else {
            $("#locAddress").val("");
            $("#Loc1Latitude").val("");
            $("#Loca1Longitude").val("");
        }
    });

    $(document).off("click", "#backButtonToEnvironmental");
    $(document).on("click", "#backButtonToEnvironmental", function (e) {
        e.preventDefault();
        $("#pills-safety-tab").trigger("click");
    });

    $(document).off("click", "#backButtonToSeverity");
    $(document).on("click", "#backButtonToSeverity", function (e) {
        e.preventDefault();
        $("#pills-severity-tab").trigger("click");
    });

    $(document).off("click", "#backButtonToDescription");
    $(document).on("click", "#backButtonToDescription", function (e) {
        e.preventDefault();
        $("#pills-description-tab").trigger("click");
    });

    $(document).off("click", "#backButtonToIncidentDtl");
    $(document).on("click", "#backButtonToIncidentDtl", function (e) {
        e.preventDefault();
        $("#pills-detail-tab").trigger("click");
    });

    $(document).off("click", "#backButtonToIncidentLoc");
    $(document).on("click", "#backButtonToIncidentLoc", function (e) {
        e.preventDefault();
        $("#pills-location-tab").trigger("click");
    });

    $(document).off("click", "#backButtonToCaller");
    $(document).on("click", "#backButtonToCaller", function (e) {
        e.preventDefault();
        $("#pills-caller-tab").trigger("click");
    });

    $(document).off("click", "#backButtonToAdditionalLoc");
    $(document).on("click", "#backButtonToAdditionalLoc", function (e) {
        e.preventDefault();
        $("#pills-additional-tab").trigger("click");
    });

    // Open Add
    // Add new
    $(document).on("click", "#btnAddIncident", function () {
        resetIncidentForm();
        LoadIncidentModal();   // load modal
    });

    // Edit
    $(document).on("click", ".btnEditIncident", function () {
        const id = $(this).attr('id');
        resetIncidentForm();
        LoadIncidentModal(id);   // load modal
    });


    $(document).off("change", "#IsOtherEvent");
    $(document).on("change", "#IsOtherEvent", function (e) {
        if ($(this).is(":checked")) {
            $(".OtherEventDetail").show();
            $(".eventCheck input[type='checkbox']").prop("checked", false);
        }
        else {
            $("#OtherEventDetail").val('');
            $(".OtherEventDetail").hide();
        }
    });

    $(document).off("keyup", "#global_search_value");
    $(document).on("keyup", "#global_search_value", function (e) {
        var description = $(this).val().trim();
        var statusID = $("#statusSelect").val() != "" ? $("#statusSelect").val() : 0;
        var severityID = $("#severitySelect").val() != "" ? $("#severitySelect").val() : 0;

        e.preventDefault();

        if (description.length >= 3) {
            GetIncidentList(statusID, severityID, description);
        }
        else {
            GetIncidentList(statusID, severityID, "");
        }
    });

    $(document).off("change", ".eventCheck input[type='checkbox']");
    $(document).on("change", ".eventCheck input[type='checkbox']", function () {
        if ($(".eventCheck input[type='checkbox']:checked").length > 0) {
            $(".OtherEventDetail").hide();
            $("#OtherEventDetail").val('');
            $("#IsOtherEvent").prop("checked", false); // uncheck if needed
        }
    });


    $(document).off("click", ".view-incident");
    $(document).on("click", ".view-incident", function () {
        var id = $(this).data("id");
        $.get("/Incidents/GetIncidentDetails", { id: id }, function (data) {

            $("#incidentDetailModalContainer").empty().html(data);

            // Show Bootstrap modal
            $("#incidentDetailsModal").modal("show");

        });
    });
    $(document).on("click", ".note-incident", function () {
        var id = $(this).data("id");
        $.get("/IncidentNotes/GetNotesModal", { incidentId: id }, function (data) {
            $("#incidentNotesModalContainer").html(data);
            $("#noteIncidentModal").modal("show");
        });
    });

    $(document).on("submit", "#addNoteForm", function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: "/IncidentNotes/AddNote",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (html) {
                // ✅ Replace only the notes log
                $("#notesLogContainer").html(html);

                // Reset form
                $("#addNoteForm")[0].reset();
            },
            error: function () {
                alert("Error while adding note.");
            }
        });
    });

    $(document).on("click", ".historyIcon", function () {
        var id = $(this).data("id");

        $.get("/IncidentHistory/GetHistoryModal", { incidentId: id }, function (data) {
            // Inject modal content into container
            $("#incidentHistoryContainer").html(data);

            // Show modal
            $("#historyIncidentModal").modal("show");
        });
    });



    // Show error: red border + message
    function showError($field) {
        $field.css("border", "1px solid red");
    }

    // Clear error: reset border + remove message
    function clearError($field) {
        $field.css("border", ""); // reset to default
        $field.siblings(".field-validation-error").remove();
    }
});
function ShowImage(input) {
    if (input.files && input.files.length > 0) {
        var preview = $('#image-thumbnails');
        preview.empty(); // clear previous thumbnails if needed

        Array.from(input.files).forEach(function (file) {
            if (file.type.match('image.*')) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var img = $('<img>')
                        .attr('src', e.target.result)
                        .addClass('img-thumbnail me-2 mb-2')
                        .css({ 'max-width': '100px', 'max-height': '100px' });
                    preview.append(img);
                };
                reader.readAsDataURL(file);
            } else {
                var fileIcon = $('<div>')
                    .addClass('file-item me-2 mb-2 p-2 border rounded')
                    .html('<i class="fa fa-file"></i> ' + file.name);
                preview.append(fileIcon);
            }
        });
    }
}
function RemoveImage() {
    document.getElementById("Image").value = "";
    $(".img-thumbnail").remove();
    $("#hidden-image-url").val(null);
}

async function SaveIncidentForm() {
    try {

        $("#statusSelect").val('');
        $("#severitySelect").val('');
        let form = [];
        let formData = new FormData();
        let obj = $("#NewIncidentForm")[0];

        // Add files
        $(obj).find("input[type='file']").each(function (i, tag) {
            for (let file of tag.files) {
                formData.append(tag.name, file);
                form.push({ name: tag.name, file: file });
            }
        });

        // Serialize other fields
        let params = $(obj).serializeArray();
        let assetIds = [];
        let eventTypeIds = [];
        $.each(params, function (i, val) {
            if (val.name === "asset.Id") {
                assetIds.push(val.value);
                form.push({ name: val.name, value: val.value });
            }
            else if (val.name === "incidentDetails.EventTypeIds") {
                eventTypeIds.push(val.value);
                form.push({ name: val.name, value: val.value });
            }
            else if (val.name === "incidentDetails.IsOtherEvent") {
                formData.append("incidentDetails.IsOtherEvent", $("#IsOtherEvent").is(":checked"));
                form.push({ name: val.name, value: $("#IsOtherEvent").is(":checked") });
            }
            else if (val.name === "incidentCellerInformation.CallTime") {
                formData.append("incidentCellerInformation.CallTime", $("#hdnCallTimedatetime").val());
                form.push({ name: val.name, value: $("#hdnCallTimedatetime").val() });
            }

            else {
                formData.append(val.name, val.value);
                form.push({ name: val.name, value: val.value });
            }
        });
        const data = sessionStorage.getItem("addLocationFormDataList");
        if (data) {
            const locations = JSON.parse(data);
            locations.forEach(function (loc, idx) {
                let assets = Array.isArray(loc.AssetIDs) ? loc.AssetIDs.join(", ") : "";
                formData.append(`AdditionalLocations[${idx}].LocationAddress`, loc.LocationAddress);
                formData.append(`additionalLocations[${idx}].Latitude`, loc.Latitude);
                formData.append(`additionalLocations[${idx}].Longitude`, loc.Longitude);
                formData.append(`additionalLocations[${idx}].NearestIntersection`, loc.NearestIntersection);
                formData.append(`additionalLocations[${idx}].ServiceAccount`, loc.ServiceAccount);
                formData.append(`additionalLocations[${idx}].PerimeterTypeDigit`, loc.PerimeterTypeDigit);
                formData.append(`additionalLocations[${idx}].AssetIDs`, assets);
                // $list.append(`
                //     <div class="card mb-2 p-2 position-relative">
                //         <button type="button" class="btn btn-link text-danger position-absolute top-0 end-0 delete-location-btn" data-idx="${idx}" title="Delete" style="font-size:1.2rem;">
                //             <i class="fa fa-trash"></i>
                //         </button>
                //         <div><strong>Address:</strong> ${loc.LocationAddress || ""}</div>
                //         <div><strong>Latitude:</strong> ${loc.Latitude || ""}</div>
                //         <div><strong>Longitude:</strong> ${loc.Longitude || ""}</div>
                //         <div><strong>Nearest:</strong> ${loc.NearestIntersection || ""}</div>
                //         <div><strong>Assets:</strong> ${assets}</div>
                //     </div>
                // `);
            });
            sessionStorage.clear();
        }

        //$.each(params, function (i, val) {
        //    if (val.name === "eventTypes.Id") {
        //        eventTypeIds.push(val.value);
        //        form.push({ name: val.name, value: val.value });
        //    } else {
        //        formData.append(val.name, val.value);
        //        form.push({ name: val.name, value: val.value });
        //    }
        //});


        // Add AssetIds
        if (assetIds.length > 0) {
            formData.append("incidentiLocation.AssetIDs", assetIds.join(","));
        }

        // Add EventTypes
        if (eventTypeIds.length > 0) {
            formData.append("incidentDetails.EventTypeIds", eventTypeIds.join(","));
        }

        showLoader($("#addIncidentModal"));

        //console.log(formData);
        console.log(form);

        // Send request
        let response = await fetch("/Incidents/SaveIncident", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            SwalSuccessAlert(result.data);
            $(".btn-close").trigger("click");
            GetIncidentList(0, 0, "");
        } else {
            SwalErrorAlert(result.message || "Failed to save incident.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving incident!");
        console.error(error);
    } finally {
        hideLoader($("#addIncidentModal"));
    }
}

async function GetIncidentList(statusID, severityID, description) {
    try {
        let payload = { severityId: severityID, statusId: statusID, description: description };

        showLoader($(".main-content"));

        const response = await fetch("/Incidents/GetIncidentList", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error("Failed to load incident list");

        const content = await response.text();
        $("#incidentGrid").empty().html(content);

    } catch (error) {
        console.error("Error loading incident list:", error);
    } finally {
        hideLoader($(".main-content"));
        // find your Date/Time header by its text
        const $dateHeader = $("th:contains('Date/Time')");
        let ascending = true;

        $dateHeader.css("cursor", "pointer");

        $dateHeader.on("click", function () {

            const $table = $(this).closest("table");
            const $tbody = $table.find("tbody");
            const $rows = $tbody.find("tr").has("td"); // skip "no records" row

            $rows.sort(function (a, b) {
                const aText = $(a).find("td:eq(10)").text().trim(); // adjust index if needed
                const bText = $(b).find("td:eq(10)").text().trim();

                const aDateTime = parseDateTime(aText);
                const bDateTime = parseDateTime(bText);

                if (aDateTime < bDateTime) return ascending ? -1 : 1;
                if (aDateTime > bDateTime) return ascending ? 1 : -1;
                return 0;
            });

            $.each($rows, function (index, row) {
                $tbody.append(row); // reattach sorted rows
            });

            ascending = !ascending;

            // update icon if exists
            const $icon = $dateHeader.find("i");
            if ($icon.length) {
                $icon.removeClass("fa-sort fa-sort-up fa-sort-down");
                $icon.addClass(ascending ? "fa-sort-up" : "fa-sort-down");
            }
        });

        // helper for "15 Sep, 2025 14:20 PM" format
        function parseDateTime(dateTimeStr) {
            // remove commas
            dateTimeStr = dateTimeStr.replace(",", "").trim();

            // Split into date and time
            // e.g. "15 Sep 2025 14:20 PM"
            const parts = dateTimeStr.split(" ");
            if (parts.length < 4) return new Date(dateTimeStr);

            const [day, monthStr, year, timeWithAmPm] = parts;
            const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

            const month = monthNames.indexOf(monthStr) + 1;
            const time = timeWithAmPm.toUpperCase();

            // build "YYYY-MM-DD HH:mm AM/PM"
            return new Date(`${year}-${month}-${day} ${time}`);
        }

    }
}

async function LoadIncidentModal(id = 0) {
    try {
        showLoader($(".main-content"));

        const url = id > 0
            ? `/Incidents/EditIncident?id=${id}`   // Edit mode
            : `/Incidents/AddIncident`;            // Add mode

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident modal");

        const content = await response.text();
        $("#incidentAddEditModalContainer").empty().html(content);

        // Show Bootstrap modal
        $("#addIncidentModal").modal("show");

    } catch (error) {
        console.error("Error loading incident modal:", error);
    } finally {
        hideLoader($(".main-content"));
        maskTelephone(".input-telephone");
        $("#addIncidentModalModalLabel").text(id > 0 ? "Update Incident" : "Add Incident");

        //if (id > 0) {
        //    displaySavedAttachments($.makeArray($("#hidden-image-url").val()));
        //}
    }
}
function resetIncidentForm() {
    const form = document.querySelector("#NewIncidentForm");
    if (!form) return;

    // Reset all standard inputs
    form.reset();

    // Reset file inputs (form.reset doesn’t always clear them)
    form.querySelectorAll("input[type='file']").forEach(fileInput => {
        fileInput.value = "";
    });

    // Reset checkboxes & radios (ensure uncheck)
    form.querySelectorAll("input[type='checkbox'], input[type='radio']").forEach(input => {
        input.checked = false;
    });

    // Reset dropdowns to first option
    form.querySelectorAll("select").forEach(select => {
        select.selectedIndex = 0;
    });
}

//function displaySavedAttachments(attachmentUrls) {
//    var preview = $('#image-thumbnail');
//    preview.empty();

//    attachmentUrls.forEach(function (url) {
//        var fileName = url.split('/').pop();
//        var isImage = /\.(jpg|jpeg|png|gif|bmp|webp)$/i.test(fileName);

//        if (isImage) {
//            fetch(url)
//                .then(response => response.blob())
//                .then(blob => {
//                    var reader = new FileReader();
//                    reader.onloadend = function () {
//                        var img = $('<img>').attr('src', reader.result)
//                            .addClass('img-thumbnail me-2 mb-2')
//                            .css({ 'max-width': '100px', 'max-height': '100px' });
//                        preview.append(img);
//                    };
//                    reader.readAsDataURL(blob); // Convert blob -> Base64
//                })
//                .catch(err => {
//                    console.error("Error loading image:", err);
//                });
//        } else {
//            var fileIcon = $('<div>').addClass('file-item me-2 mb-2 p-2 border rounded')
//                .html('<i class="fa fa-file"></i> ' + fileName.split('_').slice(1).join('_'));
//            preview.append(fileIcon);
//        }
//    });
//}

//async function ChangeIncidentStatus(incidentID, statusID) {
//    try {
//        showLoader($(".main-content"));

//        // Prepare request payload
//        let payload = {
//            incidentId: incidentID,
//            status: statusID
//        };

//        // Send request
//        let response = await fetch("/Incidents/ChangeIncidentStatus", {
//            method: "POST",
//            headers: {
//                "Content-Type": "application/json"
//            },
//            body: JSON.stringify(payload)
//        });

//        let result = await response.json();

//        if (response.ok && result.success) {
//            SwalSuccessAlert(result.data || "Status updated successfully.");
//            GetIncidentList(0, 0, "");
//        } else {
//            SwalErrorAlert(result.message || "Failed to change status of incident.");
//        }
//    } catch (error) {
//        SwalErrorAlert("Error while changing status of incident!");
//        console.error(error);
//    } finally {
//        hideLoader($(".main-content"));
//    }
//}
//function ShowImage(input) {
//    if (input.files && input.files[0]) {
//        var reader = new FileReader();
//        reader.onload = function (e) {
//            $('#image-thumbnail').attr('src', e.target.result);
//        }
//        reader.readAsDataURL(input.files[0]);
//    }
//}
