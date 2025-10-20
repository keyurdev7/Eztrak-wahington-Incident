$(function () {
    GetCloseOutDetails();
    GetRepairDetails();
    $(document).off("change", "#ddlStatus, #ddlOwner");
    $(document).on("change", "#ddlStatus, #ddlOwner", function (e) {

        var statusID = $("#ddlStatus").val() != "" ? $("#ddlStatus").val() : 0;
        var ownerId = $("#ddlOwner").val() != "" ? $("#ddlOwner").val() : 0;
        var step = $("#global_search_value").val() != "" ? $("#global_search_value").val() : "";

        e.preventDefault();
        GetAssessmentDetails(statusID, ownerId, step);
    });

    $(document).off("keyup", "#global_search_value");
    $(document).on("keyup", "#global_search_value", function (e) {
        var step = $(this).val().trim();
        var statusID = $("#ddlStatus").val() != "" ? $("#ddlStatus").val() : 0;
        var ownerId = $("#ddlOwner").val() != "" ? $("#ddlOwner").val() : 0;

        e.preventDefault();

        if (step.length >= 3) {
            GetAssessmentDetails(statusID, ownerId, step);
        }
        else {
            GetAssessmentDetails(statusID, ownerId, "");
        }
    });

    $(document).off("click", "#btnUpdateAssessment");
    $(document).on("click", "#btnUpdateAssessment", async function (e) {

        showLoader($("#updateIncidentAssestmentModal"));

        e.preventDefault();

        const formData = new FormData();

        // Collect basic fields
        formData.append("Id", document.getElementById("assessmentId").value);
        formData.append("StatusId", document.getElementById("status").value);
        formData.append("AssigneeId", document.getElementById("assignee").value);
       // formData.append("StartedTime", document.getElementById("startedTime").value);
        //formData.append("CompletedTime", document.getElementById("completedTime").value);
        formData.append("Description", document.getElementById("description").value);
        formData.append("MainStepId", document.getElementById("mainstepId").value);
        formData.append("SubStepId", document.getElementById("substepId").value);
        formData.append("IncidentId", document.getElementById("hdnIncidentID").value);
        formData.append("ImageUrl", document.getElementById("hdnImgUrl").value);

        // Append files (multiple)
        const files = document.getElementById("fileInputAssestment").files;
        for (let i = 0; i < files.length; i++) {
            formData.append("Files", files[i]);
        }

        try {
            const response = await fetch("/IncidentDetail/UpdateAssessment", {
                method: "POST",
                body: formData
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    var openTaskCount = result.asssetDetails.OpenTaskCount;
                    var completedTaskCount = result.asssetDetails.CompletedTaskCount;

                    $("#assessment").find("#openTaskCount").text(openTaskCount);
                    $("#assessment").find("#completedTaskCount").text(completedTaskCount);

                    SwalSuccessAlert("Updated Successfully");

                    // Optional: close modal and refresh table
                    $("#updateIncidentAssestmentModal").modal("hide");

                    var statusID = $("#ddlStatus").val() != "" ? $("#ddlStatus").val() : 0;
                    var ownerId = $("#ddlOwner").val() != "" ? $("#ddlOwner").val() : 0;
                    var step = $("#global_search_value").val() != "" ? $("#global_search_value").val() : "";


                    GetAssessmentDetails(statusID, ownerId, step);

                    if (result.partials) {
                        $("#div_Attachments").empty().html(result.partials.viewattachment);
                    }

                    hideLoader($("#updateIncidentAssestmentModal"));



                } else {
                    SwalErrorAlert(result.message || "Update failed.");
                    hideLoader($("#updateIncidentAssestmentModal"));
                }
            } else {
                SwalErrorAlert(result.message || "Update failed.");
                hideLoader($("#updateIncidentAssestmentModal"));
            }
            hideLoader($("#updateIncidentAssestmentModal"));
        } catch (error) {
            console.error("Error:", error);
            SwalErrorAlert(result.message || "Update failed.");
            hideLoader($("#updateIncidentAssestmentModal"));
        }
    });

    $(document).off("change", "#fileInputAssestment");
    $(document).on("change", "#fileInputAssestment", function () {
        const $previewContainer = $('#previewContainerAssestment');
        $previewContainer.empty(); // Clear previous previews

        const files = Array.from(this.files); // Convert FileList to array

        files.forEach(file => {
            const reader = new FileReader();

            reader.onload = function (e) {
                const $preview = $('<div class="preview"></div>').css({
                    width: '100px',
                    height: '100px',
                    overflow: 'hidden',
                    border: '1px solid #ddd',
                    borderRadius: '5px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    marginRight: '8px'
                }).append(`<img src="${e.target.result}" alt="Image Preview" style="max-width:100%; max-height:100%;">`);

                $previewContainer.append($preview);
            };

            reader.readAsDataURL(file);
        });
    });

    $(document).off("click", "#btnAddAssessment");
    $(document).on("click", "#btnAddAssessment", async function (e) {
        e.preventDefault();
        await SubmitAssestment();
    });


    $(document).off("click", "#btnUpdateRestoration");
    $(document).on("click", "#btnUpdateRestoration", async function (e) {


        showLoader($("#updateIncidentRestorationModal"));

        e.preventDefault();

        const formData = new FormData();

        // Collect basic fields
        formData.append("Id", document.getElementById("restorationId").value);
        formData.append("StatusId", document.getElementById("status").value);
        formData.append("RoleIds", document.getElementById("hdn_UpdateResortationRole").value);
        //formData.append("Started", document.getElementById("startedTime").value);
       // formData.append("Completed", document.getElementById("completedTime").value);
        formData.append("Description", document.getElementById("description").value);
        formData.append("Task", document.getElementById("task").value);

        //formData.append("MainStepId", document.getElementById("mainstepId").value);
        //formData.append("SubStepId", document.getElementById("substepId").value);
        formData.append("IncidentId", document.getElementById("hdnIncidentID").value);
        formData.append("ImageUrl", document.getElementById("hdnImgUrl").value);

        // Append files (multiple)
        const files = document.getElementById("fileInputRestoration").files;
        for (let i = 0; i < files.length; i++) {
            formData.append("Files", files[i]);
        }

        try {
            const response = await fetch("/IncidentDetail/UpdateRestoration", {
                method: "POST",
                body: formData
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    SwalSuccessAlert("Updated Successfully");

                    // Optional: close modal and refresh table
                    $("#updateIncidentRestorationModal").modal("hide");

                    if (result.partials) {
                        $("#div_RestorationAttachments").empty().html(result.partials.viewattachment);
                        $("#div_restoration_checklist").empty().html(result.partials.restoration);
                    }

                    hideLoader($("#updateIncidentRestorationModal"));



                } else {
                    SwalErrorAlert(result.message || "Update failed.");
                    hideLoader($("#updateIncidentRestorationModal"));
                }
            } else {
                SwalErrorAlert(result.message || "Update failed.");
                hideLoader($("#updateIncidentRestorationModal"));
            }
            hideLoader($("#updateIncidentRestorationModal"));
        } catch (error) {
            console.error("Error:", error);
            SwalErrorAlert(result.message || "Update failed.");
            hideLoader($("#updateIncidentRestorationModal"));
        }
    });

    $(document).off("change", "#fileInputRestoration");
    $(document).on("change", "#fileInputRestoration", function () {
        const $previewContainer = $('#previewContainerRestoration');
        $previewContainer.empty(); // Clear previous previews

        const files = Array.from(this.files); // Convert FileList to array

        files.forEach(file => {
            const reader = new FileReader();

            reader.onload = function (e) {
                const $preview = $('<div class="preview"></div>').css({
                    width: '100px',
                    height: '100px',
                    overflow: 'hidden',
                    border: '1px solid #ddd',
                    borderRadius: '5px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    marginRight: '8px'
                }).append(`<img src="${e.target.result}" alt="Image Preview" style="max-width:100%; max-height:100%;">`);

                $previewContainer.append($preview);
            };

            reader.readAsDataURL(file);
        });
    });

    $(document).off("click", "#btnUpdateCloseOut");
    $(document).on("click", "#btnUpdateCloseOut", async function (e) {


        showLoader($("#updateIncidentCloseOutModal"));

        e.preventDefault();

        const formData = new FormData();

        // Collect basic fields
        formData.append("Id", document.getElementById("CloseOutId").value);
        formData.append("StatusId", document.getElementById("status").value);
        formData.append("RoleIds", document.getElementById("hdn_UpdateCloseOutRole").value);
        //formData.append("Started", document.getElementById("startedTime").value);
        //formData.append("Completed", document.getElementById("completedTime").value);
        formData.append("Description", document.getElementById("description").value);
        formData.append("Task", document.getElementById("task").value);

        //formData.append("MainStepId", document.getElementById("mainstepId").value);
        //formData.append("SubStepId", document.getElementById("substepId").value);
        formData.append("IncidentId", document.getElementById("hdnIncidentID").value);

        formData.append("ImageUrl", document.getElementById("hdnImgUrl").value);
        // Append files (multiple)
        const files = document.getElementById("fileInputCloseOut").files;
        for (let i = 0; i < files.length; i++) {
            formData.append("Files", files[i]);
        }

        try {
            const response = await fetch("/IncidentDetail/UpdateClouseOut", {
                method: "POST",
                body: formData
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    SwalSuccessAlert("Updated Successfully");

                    if (result.details) {
                        $("#closeoutUploadedDocCount").text(result.details.uploadedDocumentCount);
                    }


                    // Optional: close modal and refresh table
                    $("#updateIncidentCloseOutModal").modal("hide");

                    if (result.partials) {
                        $("#div_Attachments_Closeout").empty().html(result.partials.viewattachment);
                        $("#div_closeout_details").empty().html(result.partials.closeout);
                    }

                    hideLoader($("#updateIncidentCloseOutModal"));



                } else {
                    SwalErrorAlert(result.message || "Update failed.");
                    hideLoader($("#updateIncidentCloseOutModal"));
                }
            } else {
                SwalErrorAlert(result.message || "Update failed.");
                hideLoader($("#updateIncidentCloseOutModal"));
            }
            hideLoader($("#updateIncidentCloseOutModal"));
        } catch (error) {
            console.error("Error:", error);
            SwalErrorAlert(result.message || "Update failed.");
            hideLoader($("#updateIncidentCloseOutModal"));
        }
    });

    $(document).off("change", "#fileInputCloseOut");
    $(document).on("change", "#fileInputCloseOut", function () {
        const $previewContainer = $('#previewContainerCloseOut');
        $previewContainer.empty(); // Clear previous previews

        const files = Array.from(this.files); // Convert FileList to array

        files.forEach(file => {
            const reader = new FileReader();

            reader.onload = function (e) {
                const $preview = $('<div class="preview"></div>').css({
                    width: '100px',
                    height: '100px',
                    overflow: 'hidden',
                    border: '1px solid #ddd',
                    borderRadius: '5px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    marginRight: '8px'
                }).append(`<img src="${e.target.result}" alt="Image Preview" style="max-width:100%; max-height:100%;">`);

                $previewContainer.append($preview);
            };

            reader.readAsDataURL(file);
        });
    });

    $(document).off("click", "#btnUpdateRepair");
    $(document).on("click", "#btnUpdateRepair", async function (e) {
        showLoader($("#updateIncidentRepairModal"));

        e.preventDefault();

        const formData = new FormData();

        // Collect basic fields
        formData.append("Id", document.getElementById("repairId").value);
        formData.append("IncidentId", document.getElementById("IncidentId").value);
        formData.append("IncidentValidationId", document.getElementById("IncidentValidationId").value);
        formData.append("FieldTypeId", document.getElementById("FieldTypeId").value);
        formData.append("SOL_Path", document.getElementById("hdnImgUrl").value);

        //formData.append("StatusId", document.getElementById("FieldTypeId").value);
        if (document.getElementById("FieldTypeId").value == 1) {
            formData.append("SourceOfLeak", document.getElementById("hdn_ResponsibleRole").value);
            formData.append("SourceOfLeakStatus", document.getElementById("status").value);
            formData.append("SOL_Remark", document.getElementById("description").value);
        }
        else if (document.getElementById("FieldTypeId").value == 2) {
            formData.append("PreventFurtherOutage", document.getElementById("hdn_ResponsibleRole").value);
            formData.append("PreventFurtherOutageStatus", document.getElementById("status").value);
            formData.append("PFO_Remark", document.getElementById("description").value);
        }
        else if (document.getElementById("FieldTypeId").value == 3) {
            formData.append("VacuumTruckFitting", document.getElementById("hdn_ResponsibleRole").value);
            formData.append("VacuumTruckFittingStatus", document.getElementById("status").value);
            formData.append("VTF_Remark", document.getElementById("description").value);
        }


        // Append files (multiple)
        const files = document.getElementById("fileInputRepair").files;
        for (let i = 0; i < files.length; i++) {
            formData.append("Files", files[i]);
        }

        try {
            const response = await fetch("/IncidentDetail/UpdateRepair", {
                method: "POST",
                body: formData
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    //var openTaskCount = result.asssetDetails.OpenTaskCount;
                    //var completedTaskCount = result.asssetDetails.CompletedTaskCount;

                    //$("#repair").find("#openTaskCount").text(openTaskCount);
                    //$("#repair").find("#completedTaskCount").text(completedTaskCount);

                    SwalSuccessAlert("Updated Successfully");

                    //// Optional: close modal and refresh table
                    //$("#updateIncidentAssestmentModal").modal("hide");

                    //var statusID = $("#ddlStatus").val() != "" ? $("#ddlStatus").val() : 0;
                    //var ownerId = $("#ddlOwner").val() != "" ? $("#ddlOwner").val() : 0;
                    //var step = $("#global_search_value").val() != "" ? $("#global_search_value").val() : "";

                    GetRepairDetails()
                    //GetAssessmentDetails(statusID, ownerId, step);

                    //if (result.partials) {
                    //    $("#div_Attachments").empty().html(result.partials.viewattachment);
                    //}

                    hideLoader($("#updateIncidentRepairModal"));

                    $("#updateIncidentRepairModal").modal("hide");

                } else {
                    SwalErrorAlert(result.message || "Update failed.");
                    hideLoader($("#updateIncidentRepairModal"));
                }
            } else {
                debugger;
                SwalErrorAlert(result.message || "Update failed.");
                hideLoader($("#updateIncidentRepairModal"));
            }
            hideLoader($("#updateIncidentRepairModal"));
        } catch (error) {
            console.error("Error:", error);
            SwalErrorAlert(result.message || "Update failed.");
            hideLoader($("#updateIncidentRepairModal"));
        }
    });

    $(document).off("change", "#fileInputRepair");
    $(document).on("change", "#fileInputRepair", function () {
        const $previewContainer = $('#previewContainerRepair');
        $previewContainer.empty(); // Clear previous previews

        const files = Array.from(this.files); // Convert FileList to array

        files.forEach(file => {
            const reader = new FileReader();

            reader.onload = function (e) {
                const $preview = $('<div class="preview"></div>').css({
                    width: '100px',
                    height: '100px',
                    overflow: 'hidden',
                    border: '1px solid #ddd',
                    borderRadius: '5px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    marginRight: '8px'
                }).append(`<img src="${e.target.result}" alt="Image Preview" style="max-width:100%; max-height:100%;">`);

                $previewContainer.append($preview);
            };

            reader.readAsDataURL(file);
        });
    });

});

async function GetAssessmentDetails(statusID, ownerId, step) {
    try {


        let payload = {
            IncidentId: $("#hdnIncidentID").val(),
            step: step,
            statusID: statusID,
            ownerId: ownerId
        };

        showLoader($("#div_assestment_details"));

        const response = await fetch("/IncidentDetail/GetAssessmentDetails", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error("Failed to load incident list");

        const content = await response.text();
        $("#div_assestment_details").empty().html(content);

    } catch (error) {
        console.error("Error loading incident list:", error);
    } finally {
        hideLoader($("#div_assestment_details"));
    }
}

async function EditAssessmentDetails(id, mainstepId, substepId) {
    try {
        showLoader($("#div_assestment_details"));

        // Send ID as query string
        const response = await fetch(`/IncidentDetail/EditAssessmentDetails?id=${id}&mainstepId=${mainstepId}&substepId=${substepId}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident details");

        const content = await response.text();
        $("#div_assestment_modal").empty().html(content);
        $("#updateIncidentAssestmentModal").modal("show");

    } catch (error) {
        console.error("Error loading incident details:", error);
    } finally {
        hideLoader($("#div_assestment_details"));
    }
}

async function ViewAssessmentDetails(id, mainstepId, substepId) {
    try {
        showLoader($("#div_restoration_checklist"));

        // Send ID as query string
        const response = await fetch(`/IncidentDetail/ViewAssessmentDetails?id=${id}&mainstepId=${mainstepId}&substepId=${substepId}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident details");

        const content = await response.text();
        $("#div_assestment_view_modal").empty().html(content);
        $("#viewIncidentAssestmentModal").modal("show");

    } catch (error) {
        console.error("Error loading incident details:", error);
    } finally {
        hideLoader($("#div_restoration_checklist"));
    }
}

async function OpenIncidentMap(id) {

    try {
        let payload = { id: id };

        showLoader($(".main-content"));

        const url = `/Incidents/GetIncidentMapDetailsbyId?id=${id}`;

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident map");

        const content = await response.text();
        $("#incidentMapContainer").empty().html(content); // 👈 replace with your target div
        $("#MapIncidentModal").modal("show");

    } catch (error) {
        console.error("Error loading incident map:", error);
    } finally {
        hideLoader($(".main-content"));
    }
}

async function AddAssessmentDetails() {
    try {
        showLoader($("#div_assestment_details"));

        // Send ID as query string
        const response = await fetch(`/IncidentDetail/AddAssessmentDetails`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident details");

        const content = await response.text();
        $("#div_Add_assestment_modal").empty().html(content);
        $("#addIncidentAssestmentModal").modal("show");

    } catch (error) {
        console.error("Error loading incident details:", error);
    } finally {
        hideLoader($("#div_assestment_details"));
    }
}

async function SubmitAssestment() {
    try {
        showLoader($("#addIncidentAssestmentModal"))

        const formData = new FormData();
        const Assessment = {};

        function getAssignAndStatus(roleSelector, divId) {
            const section = $("#div_AddIncidentAssessmentForm").find(`${roleSelector} > #${divId}`);
            return {
                assignId: section.find("#div_Assignee #assignId").val(),
                statusId: section.find("#div_Status #status").val()
            };
        }

        const mappings = {
            IC_MCR: [".IncidentCommander", "div_CreateMCR"],
            IC_Notify: [".IncidentCommander", "div_NotifyclaimAndEngineering"],
            IC_EstablishICP: [".IncidentCommander", "div_EstablishICP"],
            FER_PCA: [".FieldEnvironmentalRepresentative", "div_Preparecontainmentarea"],
            FER_LC: [".FieldEnvironmentalRepresentative", "div_Labelcontainers"],
            EGEC_RSM: [".EngineeringAndGEC", "div_Retrievesystemmaps"],
            EGEC_MLP: [".EngineeringAndGEC", "div_Marklowpoints"],
            EGEC_ICT: [".EngineeringAndGEC", "div_Initiatecosttracking"]
        };

        $.each(mappings, function (key, [role, div]) {
            const { assignId, statusId } = getAssignAndStatus(role, div) || {};
            Assessment[`${key}_AssignId`] = assignId ?? 0;
            Assessment[`${key}_StatusId`] = statusId ?? 0;
        });

        formData.append("incidentValidationAssessment", JSON.stringify(Assessment));
        formData.append("IncidentId", $("#hdnIncidentID").val() || 0);

        const response = await fetch("/IncidentDetail/SubmitAssestment", {
            method: "POST",
            body: formData
        });

        const result = await response.json();

        if (result.success) {
            SwalSuccessAlert(result.data);
            $("#addIncidentAssestmentModal").modal("hide");

            var openTaskCount = (result && result.asssetDetails && result.asssetDetails.OpenTaskCount)
                ? result.asssetDetails.OpenTaskCount
                : 0;

            var completedTaskCount = (result && result.asssetDetails && result.asssetDetails.CompletedTaskCount)
                ? result.asssetDetails.CompletedTaskCount
                : 0;


            $("#assessment").find("#openTaskCount").text(openTaskCount);
            $("#assessment").find("#completedTaskCount").text(completedTaskCount);

            const statusID = $("#ddlStatus").val() || 0;
            const ownerId = $("#ddlOwner").val() || 0;
            const step = $("#global_search_value").val() || "";

            GetAssessmentDetails(statusID, ownerId, step);

            $(".btnAddAssessmentPopup").hide();

        } else {
            SwalErrorAlert(result.message || "Failed to save Incident Validation.");
            $(".btnAddAssessmentPopup").show();
        }
    } catch (error) {
        console.error("Error submitting assessment:", error);
        SwalErrorAlert("An unexpected error occurred while submitting assessment.");
        $(".btnAddAssessmentPopup").show();
    } finally {
        hideLoader($("#addIncidentAssestmentModal"))
    }
}

async function EditRestorationDetails(id) {
    try {
        showLoader($("#div_restoration_checklist"));

        // Send ID as query string
        const response = await fetch(`/IncidentDetail/EditRestorationDetails?id=${id}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident details");

        const content = await response.text();
        $("#div_restoration_modal").empty().html(content);
        $("#updateIncidentRestorationModal").modal("show");

    } catch (error) {
        console.error("Error loading incident details:", error);
    } finally {
        hideLoader($("#div_restoration_checklist"));
    }
}

async function ViewRestorationDetails(id) {
    try {
        showLoader($("#div_restoration_view_modal"));

        // Send ID as query string
        const response = await fetch(`/IncidentDetail/ViewRestorationDetails?id=${id}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident details");

        const content = await response.text();
        $("#div_restoration_view_modal").empty().html(content);
        $("#viewIncidentRestorationModal").modal("show");

    } catch (error) {
        console.error("Error loading incident details:", error);
    } finally {
        hideLoader($("#div_restoration_view_modal"));
    }
}

async function GetRestorationDetails() {
    try {

        let id = $("#hdnIncidentID").val();

        showLoader($("#div_restoration_checklist"));

        // Send ID as query string
        const response = await fetch(`/IncidentDetail/GetRestorationDetails?id=${id}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident list");

        const content = await response.text();
        $("#div_restoration_checklist").empty().html(content);

    } catch (error) {
        console.error("Error loading incident list:", error);
    } finally {
        hideLoader($("#div_assestment_details"));
    }
}

async function GetCloseOutDetails() {
    try {

        let id = $("#hdnIncidentID").val();

        showLoader($("#div_closeout_details"));

        const response = await fetch(`/IncidentDetail/GetClouseOutDetails?id=${id}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident list");

        const content = await response.text();
        $("#div_closeout_details").empty().html(content);

    } catch (error) {
        console.error("Error loading incident list:", error);
    } finally {
        hideLoader($("#div_closeout_details"));
    }
}

async function EditCloseOutDetails(id) {
    try {
        showLoader($("#div_closeout_details"));

        // Send ID as query string
        const response = await fetch(`/IncidentDetail/EditClouseOutDetails?id=${id}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident details");

        const content = await response.text();
        $("#div_closeout_modal").empty().html(content);
        $("#updateIncidentCloseOutModal").modal("show");

    } catch (error) {
        console.error("Error loading incident details:", error);
    } finally {
        hideLoader($("#div_closeout_details"));
    }
}

async function ViewCloseOutDetails(id) {
    try {
        showLoader($("#div_closeout_view_modal"));

        // Send ID as query string
        const response = await fetch(`/IncidentDetail/ViewClouseOutDetails?id=${id}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });

        if (!response.ok) throw new Error("Failed to load incident details");

        const content = await response.text();
        $("#div_closeout_view_modal").empty().html(content);
        $("#viewIncidentCloseOutModal").modal("show");

    } catch (error) {
        console.error("Error loading incident details:", error);
    } finally {
        hideLoader($("#div_closeout_view_modal"));
    }
}

async function GetRepairDetails() {

    try {


        let id = $("#hdnIncidentID").val();

        showLoader($("#div_repair_details"));

        const response = await fetch(`/IncidentDetail/GetRepairDetails?id=${id}`, {

            method: "POST",

            headers: {

                "Content-Type": "application/json",

                "Accept": "text/html"

            },

        });

        if (!response.ok) throw new Error("Failed to load incident list");

        const content = await response.text();

        $("#div_repair_details").empty().html(content);

    } catch (error) {

        console.error("Error loading incident list:", error);

    } finally {

        hideLoader($("#div_repair_details"));

    }

}

async function EditRepairDetails(id, RepairId, FieldType, IncidentId, IncidentValidationId) {

    try {

        showLoader($("#div_repair_details"));

        // Send ID as query string

        const response = await fetch(`/IncidentDetail/EditRepairDetails?id=${id}&RepairId=${RepairId}&FieldType=${FieldType}&IncidentId=${IncidentId}&IncidentValidationId=${IncidentValidationId}`, {

            method: "GET",

            headers: {

                "Accept": "text/html"

            }

        });

        if (!response.ok) throw new Error("Failed to load incident details");

        const content = await response.text();

        $("#div_repair_modal").empty().html(content);

        $("#updateIncidentRepairModal").modal("show");

    } catch (error) {

        console.error("Error loading incident details:", error);

    } finally {

        hideLoader($("#div_repair_details"));

    }

}

