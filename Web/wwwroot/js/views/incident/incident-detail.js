$(function () {
    // Default IncidentDetail map to satellite view.
    // This must run AFTER the ArcGIS Map instance is created in the Razor view,
    // so we wait until both switchMapView and mapInstance exist.
    (function initDefaultSatellite(retriesLeft) {
        try {
            if (typeof window.switchMapView === "function" && window.mapInstance) {
                window.switchMapView('satellite');
                return;
            }
        } catch (e) {
            // ignore
        }

        if (retriesLeft <= 0) return;
        setTimeout(function () { initDefaultSatellite(retriesLeft - 1); }, 200);
    })(25);

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
        const selectedStatus = document.querySelector('input[name="restorationStatusAction"]:checked');
        if (!selectedStatus || !selectedStatus.value) {
            SwalErrorAlert("Please select a status action: Mark Complete or Mark N/A.");
            hideLoader($("#updateIncidentRestorationModal"));
            return;
        }

        const formData = new FormData();

        // Collect basic fields
        formData.append("Id", document.getElementById("restorationId").value);
        formData.append("StatusId", selectedStatus.value);
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
        const selectedStatus = document.querySelector('input[name="closeoutStatusAction"]:checked');
        if (!selectedStatus || !selectedStatus.value) {
            SwalErrorAlert("Please select a status action: Mark Complete or Mark N/A.");
            hideLoader($("#updateIncidentCloseOutModal"));
            return;
        }

        const formData = new FormData();

        // Collect basic fields
        formData.append("Id", document.getElementById("CloseOutId").value);
        formData.append("StatusId", selectedStatus.value);
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
        formData.append("VTF_Path", document.getElementById("hdnPFOImgUrl").value);
        formData.append("PFO_Path", document.getElementById("hdnVTFImgUrl").value);

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
        // normalize optional args
        statusID = (statusID === null || statusID === undefined || statusID === "") ? 0 : statusID;
        ownerId = (ownerId === null || ownerId === undefined || ownerId === "") ? 0 : ownerId;
        step = (step === null || step === undefined) ? "" : step;

        let payload = {
            IncidentId: $("#hdnIncidentID").val() || 0,
            step: step,
            statusID: statusID,
            ownerId: ownerId
        };
        debugger
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
        updateAssessmentSummaryFromDom();

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

async function EditAssessmentTask(id) {
    try {
        showLoader($("#div_assestment_details"));
        const response = await fetch(`/IncidentDetail/EditAssessmentTask?id=${id}`, {
            method: "GET",
            headers: { "Accept": "text/html" }
        });
        if (!response.ok) throw new Error("Failed to load assessment task");
        const content = await response.text();
        $("#div_assestment_modal").empty().html(content);
        $("#updateAssessmentTaskModal").modal("show");
    } catch (error) {
        console.error("Error loading assessment task:", error);
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

async function ViewAssessmentTask(id) {
    try {
        showLoader($("#div_assestment_details"));
        const response = await fetch(`/IncidentDetail/ViewAssessmentTask?id=${id}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });
        if (!response.ok) throw new Error("Failed to load assessment task details");
        const content = await response.text();
        $("#div_assestment_view_modal").empty().html(content);
        $("#viewIncidentAssessmentTaskModal").modal("show");
    } catch (error) {
        console.error("Error loading assessment task details:", error);
    } finally {
        hideLoader($("#div_assestment_details"));
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
        updateRestorationSummaryFromDom();

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
        updateCloseoutSummaryFromDom();

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

async function ViewRepairTask(id) {
    try {
        showLoader($("#div_repair_details"));
        const response = await fetch(`/IncidentDetail/ViewRepairTask?id=${id}`, {
            method: "GET",
            headers: {
                "Accept": "text/html"
            }
        });
        if (!response.ok) throw new Error("Failed to load repair task details");
        const content = await response.text();
        $("#div_repair_view_modal").empty().html(content);
        $("#viewIncidentRepairTaskModal").modal("show");
    } catch (error) {
        console.error("Error loading repair task details:", error);
    } finally {
        hideLoader($("#div_repair_details"));
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
        updateRepairSummaryFromDom();

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

async function EditRepairTask(id) {
    try {
        showLoader($("#div_repair_details"));
        const response = await fetch(`/IncidentDetail/EditRepairTask?id=${id}`, {
            method: "GET",
            headers: { "Accept": "text/html" }
        });
        if (!response.ok) throw new Error("Failed to load repair task");
        const content = await response.text();
        $("#div_repair_modal").empty().html(content);
        $("#updateIncidentRepairModal").modal("show");
    } catch (error) {
        console.error("Error loading repair task:", error);
    } finally {
        hideLoader($("#div_repair_details"));
    }
}

// Drag and Drop functions for Assessment Tasks
let draggedElement = null;

function dragStart(event) {
    draggedElement = event.target;
    event.dataTransfer.effectAllowed = 'move';
    event.dataTransfer.setData('text/html', event.target.outerHTML);
    event.target.style.opacity = '0.5';
}

function dragOver(event) {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
    return false;
}

function drop(event) {
    event.preventDefault();
    event.stopPropagation();

    if (draggedElement != event.target.closest('tr')) {
        const tableBody = event.target.closest('tbody');
        const rows = Array.from(tableBody.querySelectorAll('tr'));
        const draggedIndex = rows.indexOf(draggedElement);
        const targetIndex = rows.indexOf(event.target.closest('tr'));

        if (draggedIndex < targetIndex) {
            tableBody.insertBefore(draggedElement, event.target.closest('tr').nextSibling);
        } else {
            tableBody.insertBefore(draggedElement, event.target.closest('tr'));
        }

        // Update the sort order on the server
        const table = event.target.closest('table');
        const tableId = table.id;
        if (tableId === 'assessmentTable') {
            updateAssessmentTaskOrder();
        } else if (tableId === 'restorationTable') {
            updateRestorationTaskOrder();
        } else if (tableId === 'closeoutTable') {
            updateCloseoutTaskOrder();
        } else if (tableId === 'repairTable') {
            updateRepairTaskOrder();
        }
    }

    draggedElement.style.opacity = '1';
    draggedElement = null;
    return false;
}

async function updateAssessmentTaskOrder() {
    const table = document.getElementById('assessmentTable');
    const rows = table.querySelectorAll('tbody tr');
    const taskIds = Array.from(rows).map(row => parseInt(row.getAttribute('data-id')));

    try {
        const response = await fetch('/IncidentDetail/UpdateAssessmentTaskOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(taskIds)
        });

        if (!response.ok) {
            throw new Error('Failed to update task order');
        }

        const result = await response.json();
        if (!result.success) {
            console.error('Failed to update task order:', result.message);
            // Optionally, refresh the table to revert changes
            GetAssessmentDetails(0, 0, '');
        }
    } catch (error) {
        console.error('Error updating task order:', error);
        // Refresh the table to revert changes
        GetAssessmentDetails(0, 0, '');
    }
}

async function updateRestorationTaskOrder() {
    const table = document.getElementById('restorationTable');
    const rows = table.querySelectorAll('tbody tr');
    const taskIds = Array.from(rows).map(row => parseInt(row.getAttribute('data-id')));

    try {
        const response = await fetch('/IncidentDetail/UpdateRestorationTaskOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(taskIds)
        });

        if (!response.ok) {
            throw new Error('Failed to update task order');
        }

        const result = await response.json();
        if (!result.success) {
            console.error('Failed to update task order:', result.message);
            // Optionally, refresh the table to revert changes
            GetRestorationDetails();
        }
    } catch (error) {
        console.error('Error updating task order:', error);
        // Refresh the table to revert changes
        GetRestorationDetails();
    }
}

async function updateCloseoutTaskOrder() {
    const table = document.getElementById('closeoutTable');
    const rows = table.querySelectorAll('tbody tr');
    const taskIds = Array.from(rows).map(row => parseInt(row.getAttribute('data-id')));

    try {
        const response = await fetch('/IncidentDetail/UpdateCloseoutTaskOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(taskIds)
        });

        if (!response.ok) {
            throw new Error('Failed to update task order');
        }

        const result = await response.json();
        if (!result.success) {
            console.error('Failed to update task order:', result.message);
            // Optionally, refresh the table to revert changes
            GetCloseOutDetails();
        }
    } catch (error) {
        console.error('Error updating task order:', error);
        // Refresh the table to revert changes
        GetCloseOutDetails();
    }
}

async function updateRepairTaskOrder() {
    const table = document.getElementById('repairTable');
    const rows = table.querySelectorAll('tbody tr');
    const taskIds = Array.from(rows).map(row => parseInt(row.getAttribute('data-id')));

    try {
        const response = await fetch('/IncidentDetail/UpdateRepairTaskOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(taskIds)
        });

        if (!response.ok) {
            throw new Error('Failed to update task order');
        }

        const result = await response.json();
        if (!result.success) {
            console.error('Failed to update task order:', result.message);
            // Optionally, refresh the table to revert changes
            GetRepairDetails();
        }
    } catch (error) {
        console.error('Error updating task order:', error);
        // Refresh the table to revert changes
        GetRepairDetails();
    }
}

function updateAssessmentSummaryFromDom() {
    const $rows = $("#assessmentTable tbody tr").filter(function () {
        return !$(this).find("td[colspan]").length;
    });
    const total = $rows.length;
    let completed = 0;
    $rows.each(function () {
        const status = ($(this).find("td:nth-child(1) .status-text").text() || "").trim().toLowerCase();
        if (status === "complete") completed++;
    });
    const open = total - completed;
    const percent = total > 0 ? Math.round((completed / total) * 100) : 0;
    $("#assessmentOpenCount").text(open);
    $("#assessmentCompletedCount").text(completed);
    $("#assessmentTotalCount").text(total);
    $("#assessmentPercent").text(percent + " %");
}

function updateRepairSummaryFromDom() {
    const $rows = $("#repairTable tbody tr").filter(function () {
        return !$(this).find("td[colspan]").length;
    });
    const total = $rows.length;
    let completed = 0;
    $rows.each(function () {
        const status = ($(this).find("td:nth-child(1) .status-text").text() || "").trim().toLowerCase();
        if (status === "complete") completed++;
    });
    const open = total - completed;
    const percent = total > 0 ? Math.round((completed / total) * 100) : 0;
    $("#repairOpenCount").text(open);
    $("#repairCompletedCount").text(completed);
    $("#repairTotalCount").text(total);
    $("#repairPercent").text(percent + " %");
}

function updateRestorationSummaryFromDom() {
    const $rows = $("#restorationTable tbody tr").filter(function () {
        return !$(this).find("td[colspan]").length;
    });
    const total = $rows.length;
    let completed = 0;
    $rows.each(function () {
        const status = ($(this).find("td:nth-child(1) .status-text").text() || "").trim().toLowerCase();
        if (status === "complete") completed++;
    });
    const open = total - completed;
    const percent = total > 0 ? Math.round((completed / total) * 100) : 0;
    $("#restorationOpenCount").text(open);
    $("#restorationCompletedCount").text(completed);
    $("#restorationTotalCount").text(total);
    $("#restorationPercent").text(percent + " %");
}

function updateCloseoutSummaryFromDom() {
    const $rows = $("#closeoutTable tbody tr").filter(function () {
        return !$(this).find("td[colspan]").length;
    });
    const total = $rows.length;
    let completed = 0;
    $rows.each(function () {
        const status = ($(this).find("td:nth-child(1) .status-text").text() || "").trim().toLowerCase();
        if (status === "complete") completed++;
    });
    const open = total - completed;
    const percent = total > 0 ? Math.round((completed / total) * 100) : 0;
    $("#closeoutOpenCount").text(open);
    $("#closeoutCompletedCount").text(completed);
    $("#closeoutTotalCount").text(total);
    $("#closeoutPercent").text(percent + " %");
}

// -------------------------------
// Verification (Import + Map dots)
// -------------------------------

function ensureVerificationLayer() {
    if (window.verificationLayer) return true;
    if (!window.mapInstance || !window.GraphicsLayer) return false;

    try {
        window.verificationLayer = new window.GraphicsLayer();
        window.mapInstance.add(window.verificationLayer);
        return true;
    } catch (e) {
        console.error("Failed to create verification layer:", e);
        return false;
    }
}

function getVerificationSymbol(status) {
    const st = (status || "Pending").toString().trim().toLowerCase();
    const isVerified = st === "verified";
    return {
        type: "simple-marker",
        style: "circle",
        color: isVerified ? "#198754" : "#dc3545", // green/red
        size: "12px",
        outline: { color: "#ffffff", width: 1 }
    };
}

async function loadVerificationLocations() {
    const incidentId = $("#hdnIncidentID").val();
    if (!incidentId) return;

    // Render table
    try {
        const resp = await fetch(`/IncidentDetail/GetVerificationLocations?incidentId=${incidentId}`, {
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });
        const json = await resp.json();
        if (!json || !json.success) return;

        const items = json.items || [];
        renderVerificationTable(items);
        renderVerificationDots(items);
    } catch (e) {
        console.error("Failed to load verification locations:", e);
    }
}

function escapeHtml(s) {
    return (s || "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

function renderVerificationTable(items) {
    const $tbody = $("#verificationLocationsTable tbody");
    $tbody.empty();

    if (!items || items.length === 0) {
        $tbody.append(`<tr><td colspan="4" class="text-muted">No imported verification locations yet.</td></tr>`);
        return;
    }

    items.forEach(function (x) {
        const status = (x.status || "Pending");
        const badgeClass = status.toString().toLowerCase() === "verified" ? "bg-success" :
            status.toString().toLowerCase() === "rejected" ? "bg-secondary" : "bg-danger";

        $tbody.append(`
            <tr data-id="${x.id}">
                <td>${escapeHtml(x.address)}</td>
                <td><span class="badge ${badgeClass}">${escapeHtml(status)}</span></td>
                <td>${escapeHtml(x.serviceAccount || "")}</td>
                <td><button type="button" class="btn btn-sm btn-outline-primary btn-verify-location" data-id="${x.id}">Verify</button></td>
            </tr>
        `);
    });
}

function renderVerificationDots(items) {
    if (!ensureVerificationLayer() || !window.Graphic) return;

    window.verificationLayer.removeAll();
    if (!items || items.length === 0) return;

    items.forEach(function (x) {
        if (!x || !x.lat || !x.lon) return;

        const point = {
            type: "point",
            latitude: x.lat,
            longitude: x.lon
        };

        const graphic = new window.Graphic({
            geometry: point,
            symbol: getVerificationSymbol(x.status),
            attributes: {
                VerificationLocationId: x.id,
                IncidentID: x.incidentId // keep compatible with existing click handler
            }
        });

        window.verificationLayer.add(graphic);
    });
}

window.openVerifyLocationModal = async function (id) {
    try {
        const resp = await fetch(`/IncidentDetail/VerifyLocation?id=${id}`, {
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });
        const html = await resp.text();
        $("#div_verification_modal").empty().html(html);
        $("#verifyLocationModal").modal("show");
    } catch (e) {
        console.error("Failed to open verify modal:", e);
    }
};

$(document).off("click", "#btnImportVerificationLocations");
$(document).on("click", "#btnImportVerificationLocations", async function () {
    const incidentId = $("#hdnIncidentID").val();
    const fileInput = document.getElementById("verificationImportFile");
    const file = fileInput && fileInput.files && fileInput.files[0] ? fileInput.files[0] : null;

    if (!incidentId || !file) {
        SwalErrorAlert("Please select an Excel file to import.");
        return;
    }

    const fd = new FormData();
    fd.append("incidentId", incidentId);
    fd.append("file", file);

    try {
        const resp = await fetch(`/IncidentDetail/ImportVerificationLocations?incidentId=${incidentId}`, {
            method: "POST",
            body: fd
        });
        const json = await resp.json();
        if (json && json.success) {
            SwalSuccessAlert(`Imported ${json.imported} location(s).`);
            fileInput.value = "";
            await loadVerificationLocations();
        } else {
            SwalErrorAlert((json && json.message) || "Import failed.");
        }
    } catch (e) {
        console.error(e);
        SwalErrorAlert("Import failed.");
    }
});

$(document).off("click", ".btn-verify-location");
$(document).on("click", ".btn-verify-location", function () {
    const id = $(this).data("id");
    if (id) window.openVerifyLocationModal(id);
});

$(document).off("click", "#btnUpdateVerificationLocation");
$(document).on("click", "#btnUpdateVerificationLocation", async function () {
    const id = $("#verificationLocationId").val();
    const incidentId = $("#verificationIncidentId").val();
    const status = $("#verificationStatus").val();
    const notes = $("#verificationNotes").val();
    const serviceAccount = $("#verificationServiceAccount").val();

    // multi-select assets -> comma-separated
    const assetIds = ($("#verificationAssetIds").val() || []).join(",");

    const fd = new FormData();
    fd.append("Id", id);
    fd.append("IncidentId", incidentId);
    fd.append("VerificationStatus", status);
    fd.append("VerificationNotes", notes);
    fd.append("ServiceAccount", serviceAccount);
    fd.append("AssetIDs", assetIds);

    const files = document.getElementById("verificationFiles").files;
    for (let i = 0; i < files.length; i++) {
        fd.append("Files", files[i]);
    }

    try {
        const resp = await fetch("/IncidentDetail/UpdateVerificationLocation", {
            method: "POST",
            body: fd
        });
        const json = await resp.json();
        if (json && json.success) {
            SwalSuccessAlert("Verification saved.");
            $("#verifyLocationModal").modal("hide");
            await loadVerificationLocations();
        } else {
            SwalErrorAlert((json && json.message) || "Save failed.");
        }
    } catch (e) {
        console.error(e);
        SwalErrorAlert("Save failed.");
    }
});

// Load verification points after page init (poll until ArcGIS map is ready)
(function initVerification(retriesLeft) {
    if (ensureVerificationLayer()) {
        loadVerificationLocations();
        return;
    }
    if (retriesLeft <= 0) return;
    setTimeout(function () { initVerification(retriesLeft - 1); }, 250);
})(40);

