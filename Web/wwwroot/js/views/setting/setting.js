

$(function () {
    GetAllRelationships();

    // start setting tabs
    $(document).off("click", ".settingAllTab");
    $(document).on("click", ".settingAllTab", function (e) {
        e.preventDefault();
        var tab = $(this).attr("data-tab");
        if (tab === "source") {
            GetAllRelationships();
        }
        else if (tab === "event") {
            GetAllEventTypes();
        }
        else if (tab === "severity") {
            GetAllSeverity();
        }
        else if (tab === "status") {
            GetAllStatusLegend();
        }
        else if (tab === "asset") {
            GetAllAssetIds();
        }
        else if (tab === "type") {
            GetAllAssetTypes();
        }
        else if (tab === "Ipolicies") {
            GetAllPolicies();
        }
        else if (tab === "teamManagement") {
            $(".teamsidebar ul li:eq(0)").trigger('click');
            $(".teamsidebar ul li:eq(0)").addClass("active");
        }
        else if (tab === "user") {
            GetAllUserManagement();
        }
        else if (tab === "progress") {
            GetAllProgress();
        }
        else if (tab === "Imaterials") {
            GetAllMaterials();

        }
        else if (tab === "equipmentfields") {
            GetAllEquipmentFields();
        }
        else if (tab === "incidentRole") {
            GetAllIncidentRoles();
        }
        else if (tab === "company") {
            GetAllCompany();
        }
        else if (tab === "incidentShift") {
            GetAllIncidentShifts();
        }
    });

    $(document).off("click", ".teamAllTab");
    $(document).on("click", ".teamAllTab", function (e) {
        e.preventDefault();
        var tab = $(this).attr("data-tab");
        if (tab === "Iteams") {
            GetAllIncidentTeams();
        }
        else if (tab === "Iusers") {
            GetAllUsers();
        }
    });
    // end setting tabs

    // Start Source
    $(document).off("click", ".btnAddNewSource");
    $(document).on("click", ".btnAddNewSource", function (e) {
        e.preventDefault();
        AddRelationships();
    });

    $(document).off("click", ".btnAddNewUserManagement");
    $(document).on("click", ".btnAddNewUserManagement", function (e) {
        e.preventDefault();
        AddUserManagement();
    });

    $(document).off("click", ".cancelusermanagement");
    $(document).on("click", ".cancelusermanagement", function (e) {
        e.preventDefault();
        $("#addUserManagement").empty().html('');
        $('li.active').trigger('click')
    });

    $(document).off("click", ".deleteusermanagement");
    $(document).on("click", ".deleteusermanagement", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteUserManagement(id);
    });

    $(document).off("click", ".editusermanagement");
    $(document).on("click", ".editusermanagement", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetUserManagementById(id);
    });

    $(document).on("click", ".saveusermanagement", function (e) {
        e.preventDefault();
        var isValid = true;
        if ($("#RoleId").val() === "" || $("#RoleId").val() === null) {
            SwalErrorAlert("Please Select Role");
            isValid = false;
        }
        $("#addUserModal").find("input[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());
            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        if (isValid) {
            SaveUserManagement();
        }
    });


    $(document).off("click", ".cancelSource");
    $(document).on("click", ".cancelSource", function (e) {
        e.preventDefault();
        $("#addSource").empty().html('');
        $('li.active').trigger('click')
    });

    $(document).off("click", ".saveSource");
    $(document).on("click", ".saveSource", function (e) {
        e.preventDefault();
        var isValid = true;

        $("#saveSourceDiv").find("input[data-val-required]").each(function () {
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
            SaveRelationships();
        }
    });

    $(document).off("click", ".editSource");
    $(document).on("click", ".editSource", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetRelationshipById(id);
    });

    $(document).off("click", ".deleteSource");
    $(document).on("click", ".deleteSource", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteSourceItem(id);
    });

    // End Source

    //Start Material

    $(document).off("click", ".saveMaterial");
    $(document).on("click", ".saveMaterial", function (e) {
        e.preventDefault();
        if (validateMaterialForm()) SaveMaterial();
    });

    $(document).off("click", ".cancelMaterial");
    $(document).on("click", ".cancelMaterial", function (e) {
        e.preventDefault();
        $("#addMaterial").empty();
        $('li.active').trigger('click');
    });

    // any other initialization: select2, masks, etc.


    /* Confirmation + delete helper */
    function DeleteMaterialItem(id) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "No, cancel!",
            confirmButtonClass: 'btn btn-success me-2',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function (result) {
            if (result.isConfirmed) {
                DeleteMaterialById(id);
            }
        });
    }

    /* Event wiring for list actions (edit/delete/add) */
    $(document).off("click", ".btnAddNewMaterial");
    $(document).on("click", ".btnAddNewMaterial", function (e) {
        e.preventDefault();
        AddMaterial();
    });

    $(document).off("click", ".editMaterial");
    $(document).on("click", ".editMaterial", function (e) {
        e.preventDefault();
        const id = $(this).attr("id");
        if (id) GetMaterialById(id);
    });

    $(document).off("click", ".deleteMaterial");
    $(document).on("click", ".deleteMaterial", function (e) {
        e.preventDefault();
        const id = $(this).attr("id");
        if (!id) return;
        DeleteMaterialItem(id);
    });

    // Start Event Type
    $(document).off("click", ".btnAddNewEvent");
    $(document).on("click", ".btnAddNewEvent", function (e) {
        e.preventDefault();
        AddEventType();
    });

    $(document).off("click", ".cancelEventType");
    $(document).on("click", ".cancelEventType", function (e) {
        e.preventDefault();
        $("#addEventType").empty().html('');
        $('li.active').trigger('click')
    });

    $(document).off("click", ".saveEventType");
    $(document).on("click", ".saveEventType", function (e) {
        e.preventDefault();
        var isValid = true;

        $("#saveEventTypeDiv").find("input[data-val-required], textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        // ✅ only run once after validation check
        if (isValid) {
            SaveEventType();
        }
    });

    $(document).off("click", ".editEventType");
    $(document).on("click", ".editEventType", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetEventTypeById(id);
    });

    $(document).off("click", ".deleteEventType");
    $(document).on("click", ".deleteEventType", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");

        //e.preventDefault();
        DeleteEventTypeItem(id);
    });

    // End Event Type

    // Start Statuslegend

    $(document).off("click", ".btnAddNewstatusLegend");
    $(document).on("click", ".btnAddNewstatusLegend", function (e) {
        e.preventDefault();
        AddStatusLegend();
    });

    $(document).off("click", ".cancelstatusLegend");
    $(document).on("click", ".cancelstatusLegend", function (e) {
        e.preventDefault();
        $("#addstatusLegend").empty().html('');
        $('li.active').trigger('click')
    });

    $(document).off("click", ".savestatusLegend");
    $(document).on("click", ".savestatusLegend", function (e) {
        e.preventDefault();
        var isValid = true;

        $("#savestatusLegendDiv").find("input[data-val-required], textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        // ✅ only run once after validation check
        if (isValid) {
            SaveStatusLegend();
        }
    });

    $(document).off("click", ".editStatusLegend");
    $(document).on("click", ".editStatusLegend", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetStatusLegendById(id);
    });

    $(document).off("click", ".deleteStatusLegend");
    $(document).on("click", ".deleteStatusLegend", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteStatusLegendItem(id);
    });

    // End Statuslegend

    // Start Severity Level
    $(document).off("click", ".btnAddNewseverityLevl");
    $(document).on("click", ".btnAddNewseverityLevl", function (e) {
        e.preventDefault();
        AddSeverity();
    });

    $(document).off("click", ".cancelSeverityLevel");
    $(document).on("click", ".cancelSeverityLevel", function (e) {
        e.preventDefault();
        $("#addseverityLevl").empty().html('');
        $('li.active').trigger('click')
    });

    $(document).off("click", ".saveSeverityLevel");
    $(document).on("click", ".saveSeverityLevel", function (e) {
        e.preventDefault();
        var isValid = true;

        $("#saveSeverityLevelDiv").find("input[data-val-required], textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        // ✅ only run once after validation check
        if (isValid) {
            SaveSeverity();
        }
    });

    $(document).off("click", ".editSeverityLevel");
    $(document).on("click", ".editSeverityLevel", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetSeverityById(id);
    });

    $(document).off("click", ".deleteSeverityLevel");
    $(document).on("click", ".deleteSeverityLevel", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteSeverityItem(id);
    });
    // End Severity Level

    // === AssetId Handlers ===
    $(document).off("click", ".btnAddNewAsset");
    $(document).on("click", ".btnAddNewAsset", function (e) {
        e.preventDefault();
        AddAssetId();
    });

    $(document).off("click", ".cancelAsset");
    $(document).on("click", ".cancelAsset", function (e) {
        e.preventDefault();
        $("#addAsset").empty().html('');
        $('li.active').trigger('click')
    });


    $(document).on("click", ".saveAsset", function (e) {
        e.preventDefault();
        var isValid = true;
        $("#saveAssetDiv").find("input[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());
            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        if (isValid) {
            SaveAssetId();
        }
    });

    $(document).off("click", ".editAsset");
    $(document).on("click", ".editAsset", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetAssetIdById(id);
    });

    $(document).off("click", ".deleteAsset");
    $(document).on("click", ".deleteAsset", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteAssetItem(id);
    });



    $(document).off("click", ".editAssetType");
    $(document).on("click", ".editAssetType", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetAssetTypeById(id);
    });

    $(document).off("click", ".deleteAssetType");
    $(document).on("click", ".deleteAssetType", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteAssetTypeItem(id);
    });

    // === Validation Helpers ===
    function showError($field) { $field.css("border", "1px solid red"); }
    function clearError($field) { $field.css("border", ""); $field.siblings(".field-validation-error").remove(); }

    $(document).on("click", ".btnAddNewIncidentTeam", function (e) {
        e.preventDefault();
        AddIncidentTeam();
    });

    $(document).off("click", ".cancelIncidentTeam");
    $(document).on("click", ".cancelIncidentTeam", function (e) {
        e.preventDefault();
        $("#addIncidentTeam").empty().html('');
        $('li.active').trigger('click');
    });

    $(document).on("click", ".saveIncidentTeam", function (e) {
        e.preventDefault();
        var isValid = true;
        $("#saveIncidentTeamDiv").find("input[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());
            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        if (isValid) {
            SaveIncidentTeam();
        }
    });

    $(document).off("click", ".editIncidentTeam");
    $(document).on("click", ".editIncidentTeam", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetIncidentTeamById(id);
    });

    $(document).off("click", ".deleteIncidentTeam");
    $(document).on("click", ".deleteIncidentTeam", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteIncidentTeamItem(id);
    });


    $(document).off("click", ".btnAddNewPolicy");
    $(document).on("click", ".btnAddNewPolicy", function (e) {
        e.preventDefault();
        AddPolicy();
    });

    $(document).off("click", ".cancelPolicy");
    $(document).on("click", ".cancelPolicy", function (e) {
        e.preventDefault();
        $("#addPolicy").empty();
        GetAllPolicies();
        $('.teamAllTab[data-tab="Ipolicies"]').addClass('active').siblings().removeClass('active');
    });

    // delete policy
    $(document).off("click", ".deletePolicy");
    $(document).on("click", ".deletePolicy", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeletePolicyById(id);
    });

    // edit policy
    $(document).off("click", ".editPolicy");
    $(document).on("click", ".editPolicy", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetPolicyById(id);
    });

    // save policy (client-side required check + submit)
    $(document).off("click", ".savePolicy");
    $(document).on("click", ".savePolicy", function (e) {
        e.preventDefault();
        var isValid = true;

        $("#savePolicyDiv").find("input[data-val-required], textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());
            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        if (isValid) {
            SavePolicy();
        }
    }); // open add policy partial
    //$(document).off("click", ".btnAddNewPolicy");
    //$(document).on("click", ".btnAddNewPolicy", function (e) {
    //    e.preventDefault();
    //    AddPolicy();
    //});

    $(document).off("click", ".cancelPolicy");
    $(document).on("click", ".cancelPolicy", function (e) {
        e.preventDefault();
        $("#addPolicy").empty();
        GetAllPolicies();
        $('.teamAllTab[data-tab="Ipolicies"]').addClass('active').siblings().removeClass('active');
    });

    $(document).off("click", ".btnAddNewUser");
    $(document).on("click", ".btnAddNewUser", function (e) {
        e.preventDefault();
        AddUser();
    });

    $(document).off("click", ".canceluser");
    $(document).on("click", ".canceluser", function (e) {
        e.preventDefault();
        $("#addUser").empty().html('');
        $('.teamAllTab[data-tab="Iusers"]').addClass('active').siblings().removeClass('active');
    });

    $(document).off("click", ".deleteuser");
    $(document).on("click", ".deleteuser", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteUser(id);
    });

    $(document).off("click", ".edituser");
    $(document).on("click", ".edituser", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetUserById(id);
    });

    $(document).on("click", ".saveuser", function (e) {
        e.preventDefault();
        var isValid = true;
        if ($("#TeamId").val() === "" || $("#TeamId").val() === null) {
            isValid = false;
            showError($("#TeamId"));
        }
        if ($("#CompanyId").val() === "" || $("#CompanyId").val() === null) {
            isValid = false;
            showError($("#CompanyId"));
        }
        if ($("#IncidentRoleId").val() === "" || $("#IncidentRoleId").val() === null) {
            isValid = false;
            showError($("#IncidentRoleId"));
        }
        var pinhash = $("#pinhash").val();
        var verifyPin = $("#verifypin").val();
        if (pinhash !== verifyPin) {
            SwalErrorAlert("Pin and VerifyPin are not match");
            isValid = false;
        }
        $("#saveuserDiv").find("input[type='text'], select[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());
            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        if (isValid) {
            SaveUser();
        }
    });

    // Progress
    $(document).off("click", ".btnAddNewProgress");
    $(document).on("click", ".btnAddNewProgress", function (e) {
        e.preventDefault();
        AddProgress();
    });

    $(document).off("click", ".cancelProgress");
    $(document).on("click", ".cancelProgress", function (e) {
        e.preventDefault();
        $("#addProgress").empty().html('');
        $('li.active').trigger('click')
    });

    $(document).off("click", ".editProgress");
    $(document).on("click", ".editProgress", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetProgressById(id);
    });

    $(document).off("click", ".deleteProgress");
    $(document).on("click", ".deleteProgress", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteProgressItem(id);
    });

    $(document).off("click", ".saveProgress");
    $(document).on("click", ".saveProgress", function (e) {
        e.preventDefault();
        var isValid = true;

        $("#saveProgressDiv").find("input[data-val-required]").each(function () {
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
            SaveProgress();
        }
    });

    //EquipmentFields
    $(document).off("click", ".btnAddNewEquipmentFields");
    $(document).on("click", ".btnAddNewEquipmentFields", function (e) {
        e.preventDefault();
        AddEquipmentFields();
    });

    $(document).off("click", ".cancelEquipmentFields");
    $(document).on("click", ".cancelEquipmentFields", function (e) {
        e.preventDefault();
        $("#addEquipmentFields").empty().html('');
        $('li.active').trigger('click')
    });

    $(document).off("click", ".editEquipmentFields");
    $(document).on("click", ".editEquipmentFields", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetEquipmentFieldsById(id);
    });

    $(document).off("click", ".deleteEquipmentFields");
    $(document).on("click", ".deleteEquipmentFields", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteEquipmentFieldsItem(id);
    });

    $(document).off("click", ".saveEquipmentFields");
    $(document).on("click", ".saveEquipmentFields", function (e) {
        e.preventDefault();
        var isValid = true;
        $("#saveEquipmentFieldsDiv").find("input[type='text'], select[data-val-required]").each(function () {
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
            SaveEquipmentFields();
        }
    });
    // Incident Role event bindings
    $(document).off("click", ".btnAddNewIncidentRole");
    $(document).on("click", ".btnAddNewIncidentRole", function (e) {
        e.preventDefault();
        AddIncidentRole();
    });

    $(document).off("click", ".cancelIncidentRole");
    $(document).on("click", ".cancelIncidentRole", function (e) {
        e.preventDefault();
        $("#addIncidentRole").empty().html('');
        $('li.active').trigger('click');
    });

    $(document).off("click", ".saveIncidentRole");
    $(document).on("click", ".saveIncidentRole", function (e) {
        e.preventDefault();
        var isValid = true;

        $("#saveIncidentRoleDiv").find("input[data-val-required], textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        if (isValid) {
            SaveIncidentRole();
        }
    });

    $(document).off("click", ".editIncidentRole");
    $(document).on("click", ".editIncidentRole", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetIncidentRoleById(id);
    });

    $(document).off("click", ".deleteIncidentRole");
    $(document).on("click", ".deleteIncidentRole", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteIncidentItem(id);
    });


    //Company
    $(document).off("click", ".btnAddNewCompany");
    $(document).on("click", ".btnAddNewCompany", function (e) {
        e.preventDefault();
        AddCompany();
    });

    $(document).off("click", ".cancelCompany");
    $(document).on("click", ".cancelCompany", function (e) {
        e.preventDefault();
        $("#addCompany").empty().html('');
        $('li.active').trigger('click')
    });

    $(document).off("click", ".editCompany");
    $(document).on("click", ".editCompany", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetCompanyById(id);
    });

    $(document).off("click", ".deleteCompany");
    $(document).on("click", ".deleteCompany", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteCompanyItem(id);
    });

    $(document).off("click", ".saveCompany");
    $(document).on("click", ".saveCompany", function (e) {
        e.preventDefault();
        var isValid = true;
        $("#saveCompanyDiv").find("input[type='text'], select[data-val-required]").each(function () {
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
            SaveCompany();
        }
    });
    // Incident Shift event bindings
    $(document).off("click", ".btnAddNewIncidentShift");
    $(document).on("click", ".btnAddNewIncidentShift", function (e) {
        e.preventDefault();
        AddIncidentShift();
    });

    $(document).off("click", ".cancelIncidentShift");
    $(document).on("click", ".cancelIncidentShift", function (e) {
        e.preventDefault();
        $("#addIncidentShift").empty().html('');
        $('li.active').trigger('click');
    });

    $(document).off("click", ".saveIncidentShift");
    $(document).on("click", ".saveIncidentShift", function (e) {
        e.preventDefault();
        var isValid = true;

        $("#saveIncidentShiftDiv").find("input[data-val-required], textarea[data-val-required]").each(function () {
            var $field = $(this);
            var value = $.trim($field.val());

            if (value === "") {
                isValid = false;
                showError($field);
            } else {
                clearError($field);
            }
        });

        if (isValid) {
            SaveIncidentShift();
        }
    });

    $(document).off("click", ".editIncidentShift");
    $(document).on("click", ".editIncidentShift", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        GetIncidentShiftById(id);
    });

    $(document).off("click", ".deleteIncidentShift");
    $(document).on("click", ".deleteIncidentShift", function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        DeleteIncidentShiftItem(id);
    });

})

// Start Source
async function GetAllRelationships() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllRelationships", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load source list");

        const content = await response.text();
        $("#sourceList").empty().html(content);

    } catch (error) {
        console.error("Error loading source list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function AddRelationships() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/AddRelationships", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load source list");

        const content = await response.text();
        $("#addSource").empty().html(content);

    } catch (error) {
        console.error("Error loading source list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function GetRelationshipById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetRelationshipById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load source list");

        const content = await response.text();
        $("#addSource").empty().html(content);

    } catch (error) {
        console.error("Error loading source list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function DeleteRelationshipById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteRelationshipById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load source list");

        SwalSuccessAlert("Source deleted successfully!");
        GetAllRelationships();

    } catch (error) {
        console.error("Error loading source list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function SaveRelationships() {
    try {

        let form = [];
        let formData = new FormData();
        let obj = $("#NewSourceForm")[0];

        // Serialize other fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });


        showLoader($(".setting"));

        //console.log(formData);
        console.log(form);

        // Send request
        let response = await fetch("/Settings/SaveRelation", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addSource").html("");
            SwalSuccessAlert(result.data);
            GetAllRelationships();
        } else {
            SwalErrorAlert(result.message || "Failed to save relation.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving relation!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}
function DeleteSourceItem(id) {   // <-- accept id
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {   // ✅ correct way
            DeleteRelationshipById(id);
        }
    });
}
// End Source


// Start Event Type
async function GetAllEventTypes() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllEventTypes", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load event type list");

        const content = await response.text();
        $("#eventTypeList").empty().html(content);

    } catch (error) {
        console.error("Error loading event type list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function AddEventType() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/AddEventType", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load event type list");

        const content = await response.text();
        $("#addEventType").empty().html(content);

    } catch (error) {
        console.error("Error loading event type list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function GetEventTypeById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetEventTypeById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load event type list");

        const content = await response.text();
        $("#addEventType").empty().html(content);

    } catch (error) {
        console.error("Error loading event type list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function DeleteEventTypeById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteEventTypeById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load event type list");

        SwalSuccessAlert("Event Type deleted successfully!");
        GetAllEventTypes();

    } catch (error) {
        console.error("Error loading event type list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function SaveEventType() {
    try {

        let form = [];
        let formData = new FormData();
        let obj = $("#NewEventTypeForm")[0];

        // Serialize other fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });


        showLoader($(".setting"));

        //console.log(formData);
        console.log(form);

        // Send request
        let response = await fetch("/Settings/SaveEventType", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addEventType").html("");
            SwalSuccessAlert(result.data);
            GetAllEventTypes();
        } else {
            SwalErrorAlert(result.message || "Failed to save event type.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving event type!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}
function DeleteEventTypeItem(id) {   // <-- accept id
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {   // ✅ correct way
            DeleteEventTypeById(id);
        }
    });
}
// End Event Type



// Start Severity Level
async function GetAllSeverity() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllSeverity", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load severity level list");

        const content = await response.text();
        $("#severityLevlList").empty().html(content);

    } catch (error) {
        console.error("Error loading severity level list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function AddSeverity() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/AddSeverity", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to add severity level");

        const content = await response.text();
        $("#addseverityLevl").empty().html(content);

    } catch (error) {
        console.error("Failed to add severity level:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function GetSeverityById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetSeverityById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to get severity level");

        const content = await response.text();
        $("#addseverityLevl").empty().html(content);

    } catch (error) {
        console.error("Failed to get severity level:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function DeleteSeverityById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteSeverityById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to delete severity level.");

        SwalSuccessAlert("Severity level deleted successfully!");
        GetAllSeverity();

    } catch (error) {
        console.error("Failed to delete severity level:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function SaveSeverity() {
    try {

        let form = [];
        let formData = new FormData();
        let obj = $("#NewSeverityLevelForm")[0];

        // Serialize other fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });


        showLoader($(".setting"));

        //console.log(formData);
        console.log(form);

        // Send request
        let response = await fetch("/Settings/SaveSeverity", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addseverityLevl").html("");
            SwalSuccessAlert(result.data);
            GetAllSeverity();
        } else {
            SwalErrorAlert(result.message || "Failed to save severity level.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving severity level!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}
function DeleteSeverityItem(id) {   // <-- accept id
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {   // ✅ correct way
            DeleteSeverityById(id);
        }
    });
}
// End Severity Level


// Start Status legend
async function GetAllStatusLegend() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllStatusLegend", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load Status legend list");

        const content = await response.text();
        $("#statusLegendList").empty().html(content);

    } catch (error) {
        console.error("Error loading Status legend list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function AddStatusLegend() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/AddStatusLegend", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to add Status legend");

        const content = await response.text();
        $("#addstatusLegend").empty().html(content);

    } catch (error) {
        console.error("Failed to add Status legend:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function GetStatusLegendById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetStatusLegendById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to get status Legend");

        const content = await response.text();
        $("#addstatusLegend").empty().html(content);

    } catch (error) {
        console.error("Failed to get status Legend:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function DeleteStatusLegendById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteStatusLegendById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to delete status legend.");

        SwalSuccessAlert("status legend deleted successfully!");
        GetAllStatusLegend();

    } catch (error) {
        console.error("Failed to delete status legend:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function SaveStatusLegend() {
    try {

        let form = [];
        let formData = new FormData();
        let obj = $("#NewstatusLegendForm")[0];

        // Serialize other fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });


        showLoader($(".setting"));

        //console.log(formData);
        console.log(form);

        // Send request
        let response = await fetch("/Settings/SaveStatusLegend", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addstatusLegend").html("");
            SwalSuccessAlert(result.data);
            GetAllStatusLegend();
        } else {
            SwalErrorAlert(result.message || "Failed to save Status Legend.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving Status Legend!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}
function DeleteStatusLegendItem(id) {   // <-- accept id
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {   // ✅ correct way
            DeleteStatusLegendById(id);
        }
    });
}
// End Status legend


async function GetAllAssetIds() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetAllAssetIds", { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load asset list");
        const content = await response.text();
        $("#assetList").empty().html(content);
    } catch (error) { console.error("Error loading asset list:", error); }
    finally { hideLoader($(".setting")); }
}

async function AddAssetId() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/AddAssetId", { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load asset form");
        const content = await response.text();
        $("#addAsset").empty().html(content);
    } catch (error) { console.error("Error loading asset form:", error); }
    finally { hideLoader($(".setting")); }
}

async function GetAssetIdById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetAssetIdById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load asset");
        const content = await response.text();
        $("#addAsset").empty().html(content);
    } catch (error) { console.error("Error loading asset:", error); }
    finally { hideLoader($(".setting")); }
}

async function DeleteAssetIdById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/DeleteAssetIdById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to delete asset");
        SwalSuccessAlert("Asset deleted successfully!");
        GetAllAssetIds();
    } catch (error) { console.error("Error deleting asset:", error); }
    finally { hideLoader($(".setting")); }
}

async function SaveAssetId() {
    try {
        let form = [], formData = new FormData(), obj = $("#NewAssetForm")[0];
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) { formData.append(val.name, val.value); form.push({ name: val.name, value: val.value }); });
        showLoader($(".setting"));
        let response = await fetch("/Settings/SaveAssetId", { method: "POST", body: formData });
        let result = await response.json();
        if (result.success) {
            $("#addAsset").html("");
            SwalSuccessAlert(result.data);
            GetAllAssetIds();
        } else { SwalErrorAlert(result.message || "Failed to save asset."); }
    } catch (error) { SwalErrorAlert("Error while saving asset!"); console.error(error); }
    finally { hideLoader($(".setting")); }
}

function DeleteAssetItem(id) {
    Swal.fire({
        title: 'Are you sure?', text: "You won't be able to revert this!", icon: 'warning',
        showCancelButton: true, confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel!",
        confirmButtonClass: 'btn btn-success me-2', cancelButtonClass: 'btn btn-danger', buttonsStyling: false
    }).then(function (result) { if (result.isConfirmed) { DeleteAssetIdById(id); } });
}


async function GetAllAssetTypes() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetAllAssetTypes", { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load asset types");
        const content = await response.text();
        $("#assetTypeList").empty().html(content);
    } catch (error) { console.error("Error loading asset types:", error); }
    finally { hideLoader($(".setting")); }
}

async function AddAssetType() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/AddAssetType", { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load asset type form");
        const content = await response.text();
        $("#addAssetType").empty().html(content);
    } catch (error) { console.error("Error loading asset type form:", error); }
    finally { hideLoader($(".setting")); }
}

async function GetAssetTypeById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetAssetTypeById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load asset type");
        const content = await response.text();
        $("#addAssetType").empty().html(content);
    } catch (error) { console.error("Error loading asset type:", error); }
    finally { hideLoader($(".setting")); }
}

async function SaveAssetType() {
    try {
        let form = [], formData = new FormData(), obj = $("#NewAssetTypeForm")[0];
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) { formData.append(val.name, val.value); form.push({ name: val.name, value: val.value }); });
        showLoader($(".setting"));
        let response = await fetch("/Settings/SaveAssetType", { method: "POST", body: formData });
        let result = await response.json();
        if (result.success) {
            $("#addAssetType").html("");
            SwalSuccessAlert(result.data || "Asset type saved successfully!");
            GetAllAssetTypes();
        } else { SwalErrorAlert(result.message || "Failed to save asset type."); }
    } catch (error) { SwalErrorAlert("Error while saving asset type!"); console.error(error); }
    finally { hideLoader($(".setting")); }
}

async function DeleteAssetTypeById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/DeleteAssetTypeById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to delete asset type");
        SwalSuccessAlert("Asset type deleted successfully!");
        GetAllAssetTypes();
    } catch (error) { console.error("Error deleting asset type:", error); }
    finally { hideLoader($(".setting")); }
}

function DeleteAssetTypeItem(id) {
    Swal.fire({
        title: 'Are you sure?', text: "You won't be able to revert this!", icon: 'warning',
        showCancelButton: true, confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel!",
        confirmButtonClass: 'btn btn-success me-2', cancelButtonClass: 'btn btn-danger', buttonsStyling: false
    }).then(function (result) { if (result.isConfirmed) { DeleteAssetTypeById(id); } });
}

// ----------------- setting.js (paste/replace) -----------------

// Utility functions: showLoader/hideLoader/alerts assumed present in your project

// === AJAX loaders ===
async function GetAllIncidentTeams() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetAllIncidentTeams", { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load incident team list");
        const content = await response.text();
        $("#incidentTeamList").empty().html(content);
        $("#Iteams").addClass("active");
    } catch (error) { console.error("Error loading incident team list:", error); }
    finally { hideLoader($(".setting")); }
}

async function AddIncidentTeam() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/AddIncidentTeam", { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load incident team form");
        const content = await response.text();
        $("#addIncidentTeam").empty().html(content);
        if (typeof InitIncidentTeamPartial === "function") InitIncidentTeamPartial();
    } catch (error) { console.error("Error loading incident team form:", error); }
    finally { hideLoader($(".setting")); }
}

async function GetIncidentTeamById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetIncidentTeamById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load incident team");
        const content = await response.text();
        $("#addIncidentTeam").empty().html(content);
        if (typeof InitIncidentTeamPartial === "function") InitIncidentTeamPartial();
    } catch (error) { console.error("Error loading incident team:", error); }
    finally { hideLoader($(".setting")); }
}

async function DeleteIncidentTeamById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/DeleteIncidentTeamById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to delete incident team");
        SwalSuccessAlert("Incident team deleted successfully!");
        GetAllIncidentTeams();
    } catch (error) { console.error("Error deleting incident team:", error); }
    finally { hideLoader($(".setting")); }
}

async function SaveIncidentTeam() {
    try {
        // run client-side validation before anything else
        //if (!validateIncidentTeamForm()) {
        //    SwalErrorAlert("Please fix validation errors before submitting.");
        //    return;
        //}

        // Ensure existing rows are correctly indexed (so names are SpecializationList[0], [1], ...)
        $("#specializationsList .specialization-row").each(function (i) {
            $(this).find("input.specialization-input").attr("name", "SpecializationList[" + i + "]");
        });

        // Collect serialized form params
        let formObj = $("#NewIncidentTeamForm")[0];
        if (!formObj) { SwalErrorAlert("Form not found!"); return; }

        let params = $(formObj).serializeArray(); // array of {name, value}

        // If quick input has a value, add it automatically (unless it's a duplicate)
        var raw = ($("#specializationInput").val() || "").toString().trim();
        if (raw) {
            // check duplicates (case-insensitive) among existing specialization inputs
            var dup = false;
            $("#specializationsList .specialization-input").each(function () {
                var v = ($(this).val() || "").toString().trim();
                if (v && v.toLowerCase() === raw.toLowerCase()) { dup = true; return false; }
            });

            if (!dup) {
                // index for new specialization is current number of rows
                var idx = $("#specializationsList .specialization-row").length;
                // push into params so it'll be included in the FormData below
                params.push({ name: "SpecializationList[" + idx + "]", value: raw });
            } else {
                // optional: clear input if duplicate found
                // $("#specializationInput").val("");
            }
        }

        // Build FormData from params
        let formData = new FormData();
        $.each(params, function (i, val) {
            console.log("adding to formData", val.name, "=", val.value); // Debug log (optional)
            formData.append(val.name, val.value);
        });

        showLoader($(".setting"));
        let response = await fetch("/Settings/SaveIncidentTeam", { method: "POST", body: formData });
        let result = await response.json();

        if (result.success) {
            $("#addIncidentTeam").html("");
            SwalSuccessAlert(result.data);
            GetAllIncidentTeams();
        } else {
            SwalErrorAlert(result.message || "Failed to save incident team.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving incident team!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}
function validateIncidentTeamForm() {
    // ensure form exists
    const $form = $("#NewIncidentTeamForm");
    if (!$form || $form.length === 0) {
        console.warn("NewIncidentTeamForm not found for validation.");
        return false;
    }

    // If the plugin hasn't been initialized, initialize with rules.
    // Calling .validate() multiple times is safe; .form() simply validates current state.
    $form.validate({
        rules: {
            // use the actual name attributes of your inputs. Update if different.
            Name: { required: true, minlength: 3 },
            Description: { required: true }
        },
        messages: {
            Name: { required: "Team name is required", minlength: "At least 3 characters" },
            Description: { required: "Description is required" }
        },
        errorClass: "text-danger",
        errorElement: "span",
        // place errors next to bootstrap form-control (adjust selectors if using different markup)
        errorPlacement: function (error, element) {
            // if using bootstrap input-group or custom layout, adjust accordingly
            if (element.parent(".input-group").length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        },
        highlight: function (element) {
            $(element).addClass("is-invalid");
        },
        unhighlight: function (element) {
            $(element).removeClass("is-invalid");
        }
    });

    // validate and return boolean
    return $form.valid();
}

/*
 * Recommended: call this inside your partial init function so validation rules attach
 * when the partial is loaded via AddIncidentTeam / GetIncidentTeamById.
 *
 * Example partial initializer:
 */
function InitIncidentTeamPartial() {
    // attach validation rules to the partial form
    validateIncidentTeamForm();

    // any other init code: bind events for dynamic specializations, datepickers, etc.
    // e.g. $("#NewIncidentTeamForm").on("submit", function(e){ e.preventDefault(); SaveIncidentTeam(); });
}


function DeleteIncidentTeamItem(id) {
    Swal.fire({
        title: 'Are you sure?', text: "You won't be able to revert this!", icon: 'warning',
        showCancelButton: true, confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel!",
        confirmButtonClass: 'btn btn-success me-2', cancelButtonClass: 'btn btn-danger', buttonsStyling: false
    }).then(function (result) { if (result.isConfirmed) { DeleteIncidentTeamById(id); } });
}
async function GetAllPolicies() {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllPolicies", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load policy list");

        const content = await response.text();
        $("#policyList").empty().html(content);

    } catch (error) {
        console.error("Error loading policy list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function AddPolicy() {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/AddPolicy", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load add policy partial");

        const content = await response.text();
        $("#addPolicy").empty().html(content);

    } catch (error) {
        console.error("Error loading add policy partial:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function GetPolicyById(id) {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/GetPolicyById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load policy item");

        const content = await response.text();
        $("#addPolicy").empty().html(content);

    } catch (error) {
        console.error("Error loading policy item:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function DeletePolicyById(id) {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/DeletePolicyById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to delete policy");

        SwalSuccessAlert("Policy deleted successfully!");
        GetAllPolicies();

    } catch (error) {
        console.error("Error deleting policy:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function SavePolicy() {
    try {
        let form = [];
        let formData = new FormData();
        let obj = $("#NewPolicyForm")[0];

        // Serialize other fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });

        showLoader($(".setting"));

        console.log(form);

        // Send request
        let response = await fetch("/Settings/SavePolicy", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addPolicy").html("");
            SwalSuccessAlert(result.data);
            GetAllPolicies();
        } else {
            SwalErrorAlert(result.message || "Failed to save policy.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving policy!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}

function DeletePolicyItem(id) {
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {
            DeletePolicyById(id);
        }
    });
}
// End Policy

async function GetAllUserManagement() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetAllUserManagement", { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load User Management");
        const content = await response.text();
        $("#UserManagementlist").empty().html(content);
    } catch (error) { console.error("Error loading User Management list:", error); }
    finally { hideLoader($(".setting")); }
}

async function AddUserManagement() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/AddUserManagement", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load User Management");
        const content = await response.text();
        $("#addUserManagement").empty().html(content);

    } catch (error) {
        console.error("Error loading User Management:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function SaveUserManagement()  {
    try {
        let form = [], formData = new FormData(), obj = $("#NewUserManagementForm")[0];
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) { formData.append(val.name, val.value); form.push({ name: val.name, value: val.value }); });
        showLoader($(".setting"));
        let response = await fetch("/Settings/SaveUserManagement", { method: "POST", body: formData });
        let result = await response.json();
        if (result.success) {
            $("#addUserManagement").html("");
            SwalSuccessAlert(result.data);
            GetAllUserManagement();
        } else { SwalErrorAlert(result.message || "Failed to save usermanagement."); }
    } catch (error) { SwalErrorAlert("Error while saving usermanagement!"); console.error(error); }
    finally { hideLoader($(".setting")); }
}

function DeleteUserManagement(id) {
    Swal.fire({
        title: 'Are you sure?', text: "You won't be able to revert this!", icon: 'warning',
        showCancelButton: true, confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel!",
        confirmButtonClass: 'btn btn-success me-2', cancelButtonClass: 'btn btn-danger', buttonsStyling: false
    }).then(function (result) { if (result.isConfirmed) { DeleteUserManagementById(id); } });
}

async function DeleteUserManagementById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/DeleteUserManagementById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to delete usermanagement");
        SwalSuccessAlert("User Management deleted successfully!");
        GetAllUserManagement();
    } catch (error) { console.error("Error deleting usermanagement:", error); }
    finally { hideLoader($(".setting")); }
}

async function GetUserManagementById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetUserManagementById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load user Management");
        const content = await response.text();
        $("#addUserManagement").empty().html(content);
    } catch (error) { console.error("Error loading usermanagement:", error); }
    finally { hideLoader($(".setting")); }
}

async function GetAllUsers() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetAllUsers", { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load Users list");
        const content = await response.text();
        $("#Userlist").empty().html(content);
    } catch (error) { console.error("Error loading Users list:", error); }
    finally { hideLoader($(".setting")); }
}

async function AddUser() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/AddUser", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load Users list");

        const content = await response.text();
        $("#addUser").empty().html(content);

    } catch (error) {
        console.error("Error loading Users list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function SaveUser() {
    try {
        let form = [], formData = new FormData(), obj = $("#NewUserForm")[0];
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) { formData.append(val.name, val.value); form.push({ name: val.name, value: val.value }); });
        showLoader($(".setting"));
        let response = await fetch("/Settings/SaveUser", { method: "POST", body: formData });
        let result = await response.json();
        if (result.success) {
            $("#addUser").html("");
            SwalSuccessAlert(result.data);
            GetAllUsers();
        } else { SwalErrorAlert(result.message || "Failed to save user."); }
    } catch (error) { SwalErrorAlert("Error while saving user!"); console.error(error); }
    finally { hideLoader($(".setting")); }
}
function DeleteUser(id) {
    Swal.fire({
        title: 'Are you sure?', text: "You won't be able to revert this!", icon: 'warning',
        showCancelButton: true, confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel!",
        confirmButtonClass: 'btn btn-success me-2', cancelButtonClass: 'btn btn-danger', buttonsStyling: false
    }).then(function (result) { if (result.isConfirmed) { DeleteUserById(id); } });
}

async function DeleteUserById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/DeleteUserById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to delete user");
        SwalSuccessAlert("User deleted successfully!");
        GetAllUsers();
    } catch (error) { console.error("Error deleting user:", error); }
    finally { hideLoader($(".setting")); }
}

async function GetUserById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetUserById?id=" + id, { method: "GET", headers: { "Content-Type": "application/json", "Accept": "text/html" } });
        if (!response.ok) throw new Error("Failed to load user");
        const content = await response.text();
        $("#addUser").empty().html(content);
    } catch (error) { console.error("Error loading user:", error); }
    finally { hideLoader($(".setting")); }
}

async function GetAllProgress() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllProgress", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load status master list");

        const content = await response.text();
        $("#progressList").empty().html(content);

    } catch (error) {
        console.error("Error loading status master list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function AddProgress() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/AddProgress", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load status master list");

        const content = await response.text();
        $("#addProgress").empty().html(content);

    } catch (error) {
        console.error("Error loading status master list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function GetProgressById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetProgressById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load status master list");

        const content = await response.text();
        $("#addProgress").empty().html(content);

    } catch (error) {
        console.error("Error loading status master list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

function DeleteProgressItem(id) {   // <-- accept id
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {   // ✅ correct way
            DeleteProgressById(id);
        }
    });
}

async function DeleteProgressById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteProgressById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load status master list");

        SwalSuccessAlert("Status Master deleted successfully!");
        GetAllProgress();

    } catch (error) {
        console.error("Error loading status master list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function SaveProgress() {
    try {

        let form = [];
        let formData = new FormData();
        let obj = $("#NewProgressForm")[0];

        // Serialize other fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });


        showLoader($(".setting"));

        //console.log(formData);
        console.log(form);

        // Send request
        let response = await fetch("/Settings/SaveProgress", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addProgress").html("");
            SwalSuccessAlert(result.data);
            GetAllProgress();
        } else {
            SwalErrorAlert(result.message || "Failed to save status master.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving status master!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}
async function GetAllMaterials() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetAllMaterials", {
            method: "GET",
            headers: { "Content-Type": "application/json", "Accept": "text/html" }
        });
        if (!response.ok) throw new Error("Failed to load material list");
        const content = await response.text();
        $("#materialList").empty().html(content);
        $("#Materials").addClass("active");
    } catch (error) {
        console.error("Error loading material list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function AddMaterial() {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/AddMaterial", {
            method: "GET",
            headers: { "Content-Type": "application/json", "Accept": "text/html" }
        });
        if (!response.ok) throw new Error("Failed to load material form");
        const content = await response.text();
        $("#addMaterial").empty().html(content);
        if (typeof InitMaterialPartial === "function") InitMaterialPartial();
    } catch (error) {
        console.error("Error loading material form:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function GetMaterialById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/GetMaterialById?id=" + encodeURIComponent(id), {
            method: "GET",
            headers: { "Content-Type": "application/json", "Accept": "text/html" }
        });
        if (!response.ok) throw new Error("Failed to load material");
        const content = await response.text();
        $("#addMaterial").empty().html(content);
        if (typeof InitMaterialPartial === "function") InitMaterialPartial();
    } catch (error) {
        console.error("Error loading material:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function DeleteMaterialById(id) {
    try {
        showLoader($(".setting"));
        const response = await fetch("/Settings/DeleteMaterialById?id=" + encodeURIComponent(id), {
            method: "GET",
            headers: { "Content-Type": "application/json", "Accept": "application/json" }
        });
        if (!response.ok) throw new Error("Failed to delete material");
        SwalSuccessAlert("Material deleted successfully!");
        GetAllMaterials();
    } catch (error) {
        console.error("Error deleting material:", error);
        SwalErrorAlert("Error deleting material.");
    } finally {
        hideLoader($(".setting"));
    }
}

async function SaveMaterial() {
    try {
        // client validation (basic)
        if (!validateMaterialForm()) {
            SwalErrorAlert("Please fix validation errors before submitting.");
            return;
        }

        const formEl = $("#NewMaterialForm")[0];
        if (!formEl) { SwalErrorAlert("Form not found!"); return; }

        // build FormData
        const formData = new FormData(formEl);

        showLoader($(".setting"));
        const response = await fetch("/Settings/SaveMaterial", {
            method: "POST",
            body: formData
        });

        const result = await response.json();
        if (result.success) {
            $("#addMaterial").html("");
            SwalSuccessAlert(result.data || "Material saved successfully!");
            GetAllMaterials();
        } else {
            SwalErrorAlert(result.message || "Failed to save material.");
        }
    } catch (error) {
        console.error("Error saving material:", error);
        SwalErrorAlert("Error while saving material!");
    } finally {
        hideLoader($(".setting"));
    }
}

/* client-side validator for material form */
function validateMaterialForm() {
    const $form = $("#NewMaterialForm");
    if (!$form || $form.length === 0) {
        console.warn("NewMaterialForm not found for validation.");
        return false;
    }

    // simple manual validation: required fields
    let isValid = true;

    //const $materialId = $form.find("input[name='MaterialID']");
    const $name = $form.find("input[name='Name']");
    const $unitCost = $form.find("input[name='UnitCost']");

    function setInvalid($el, msg) {
        $el.addClass("is-invalid");
        if ($el.next(".invalid-feedback").length === 0) {
            $el.after(`<div class="invalid-feedback">${msg}</div>`);
        } else {
            $el.next(".invalid-feedback").text(msg);
        }
        isValid = false;
    }

    function clearInvalid($el) {
        $el.removeClass("is-invalid");
        $el.next(".invalid-feedback").remove();
    }

    //// MaterialID required
    //clearInvalid($materialId);
    //if (!$materialId.val() || !$materialId.val().toString().trim()) {
    //    setInvalid($materialId, "Material ID is required.");
    //}

    // Name required
    clearInvalid($name);
    if (!$name.val() || !$name.val().toString().trim()) {
        setInvalid($name, "Material name is required.");
    }

    // UnitCost must be non-negative integer (or zero)
    clearInvalid($unitCost);
    const uc = $unitCost.val();
    if (uc === "" || uc === null || isNaN(Number(uc)) || Number(uc) < 0) {
        setInvalid($unitCost, "Unit cost must be a non-negative number.");
    }

    return isValid;
}

/* Partial initializer - call when partial is loaded */
function InitMaterialPartial() {
    // attach simple form validation (optional: you can wire jquery.validate instead)
    $("#NewMaterialForm").off("submit").on("submit", function (e) {
        e.preventDefault();
        SaveMaterial();
    });
}


async function GetAllEquipmentFields() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllEquipmentFields", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load equipment fields list");

        const content = await response.text();
        $("#equipmentfieldsList").empty().html(content);

    } catch (error) {
        console.error("Error loading equipment fields list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function AddEquipmentFields() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/AddEquipmentFields", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load equipment fields list");

        const content = await response.text();
        $("#addEquipmentFields").empty().html(content);

    } catch (error) {
        console.error("Error loading equipment fields list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function GetEquipmentFieldsById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetEquipmentFieldsById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load equipment fields list");

        const content = await response.text();
        $("#addEquipmentFields").empty().html(content);

    } catch (error) {
        console.error("Error loading equipment fields list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

function DeleteEquipmentFieldsItem(id) {   // <-- accept id
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {   // ✅ correct way
            DeleteEquipmentFieldsById(id);
        }
    });
}

async function DeleteEquipmentFieldsById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteEquipmentFieldsById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load equipment fields list");

        SwalSuccessAlert("Equipment Field deleted successfully!");
        GetAllEquipmentFields();

    } catch (error) {
        console.error("Error loading equipment Fields list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function SaveEquipmentFields() {
    try {

        let form = [];
        let formData = new FormData();
        let obj = $("#NewEquipmentFieldsForm")[0];

        // Serialize other fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });


        showLoader($(".setting"));

        //console.log(formData);
        console.log(form);

        // Send request
        let response = await fetch("/Settings/SaveEquipmentFields", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addEquipmentFields").html("");
            SwalSuccessAlert(result.data);
            GetAllEquipmentFields();
        } else {
            SwalErrorAlert(result.message || "Failed to save equipment field.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving equipment field!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}
// Start Incident Role
async function GetAllIncidentRoles() {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllIncidentRoles", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load incident role list");

        const content = await response.text();
        $("#incidentRoleList").empty().html(content);

    } catch (error) {
        console.error("Error loading incident role list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function AddIncidentRole() {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/AddIncidentRole", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to add incident role");

        const content = await response.text();
        $("#addIncidentRole").empty().html(content);

    } catch (error) {
        console.error("Failed to add incident role:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function GetIncidentRoleById(id) {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/GetIncidentRoleById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to get incident role");

        const content = await response.text();
        $("#addIncidentRole").empty().html(content);

    } catch (error) {
        console.error("Failed to get incident role:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function DeleteIncidentRoleById(id) {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteIncidentRoleById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to delete incident role.");

        SwalSuccessAlert("Incident role deleted successfully!");
        GetAllIncidentRoles();

    } catch (error) {
        console.error("Failed to delete incident role:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function SaveIncidentRole() {
    try {
        let form = [];
        let formData = new FormData();
        let obj = $("#NewIncidentRoleForm")[0];

        // Serialize fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });

        showLoader($(".setting"));

        // Send request
        let response = await fetch("/Settings/SaveIncidentRole", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addIncidentRole").html("");
            SwalSuccessAlert(result.data);
            GetAllIncidentRoles();
        } else {
            SwalErrorAlert(result.message || "Failed to save incident role.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving incident role!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function GetAllCompany() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllCompany", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load company list");

        const content = await response.text();
        $("#CompanyList").empty().html(content);

    } catch (error) {
        console.error("Error loading company list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function AddCompany() {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/AddCompany", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load company list");

        const content = await response.text();
        $("#addCompany").empty().html(content);

    } catch (error) {
        console.error("Error loading company list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function GetCompanyById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/GetCompanyById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load company list");

        const content = await response.text();
        $("#addCompany").empty().html(content);

    } catch (error) {
        console.error("Error loading company list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}
function DeleteCompanyItem(id) {   // <-- accept id
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {   // ✅ correct way
            DeleteCompanyById(id);
        }
    });
}

async function DeleteCompanyById(id) {
    try {

        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteCompanyById?id=" + id, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
        });

        if (!response.ok) throw new Error("Failed to load company list");

        SwalSuccessAlert("Company deleted successfully!");
        GetAllCompany();

    } catch (error) {
        console.error("Error loading company list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function SaveCompany() {
    try {

        let form = [];
        let formData = new FormData();
        let obj = $("#NewCompanyForm")[0];

        // Serialize other fields
        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });


        showLoader($(".setting"));

        //console.log(formData);
        console.log(form);

        // Send request
        let response = await fetch("/Settings/SaveCompany", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addCompany").html("");
            SwalSuccessAlert(result.data);
            GetAllCompany();
        } else {
            SwalErrorAlert(result.message || "Failed to save company.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving company!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}
function DeleteIncidentItem(id) {
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {
            DeleteIncidentRoleById(id);
        }
    });
}
// End Incident Role
// Start Incident Shift
async function GetAllIncidentShifts() {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/GetAllIncidentShifts", {
            method: "GET",
            headers: { "Content-Type": "application/json", "Accept": "text/html" }
        });

        if (!response.ok) throw new Error("Failed to load incident shift list");

        const content = await response.text();
        $("#incidentShiftList").empty().html(content);
    } catch (error) {
        console.error("Error loading incident shift list:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function AddIncidentShift() {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/AddIncidentShift", {
            method: "GET",
            headers: { "Content-Type": "application/json", "Accept": "text/html" }
        });

        if (!response.ok) throw new Error("Failed to add incident shift");

        const content = await response.text();
        $("#addIncidentShift").empty().html(content);
    } catch (error) {
        console.error("Failed to add incident shift:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function GetIncidentShiftById(id) {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/GetIncidentShiftById?id=" + id, {
            method: "GET",
            headers: { "Content-Type": "application/json", "Accept": "text/html" }
        });

        if (!response.ok) throw new Error("Failed to get incident shift");

        const content = await response.text();
        $("#addIncidentShift").empty().html(content);
    } catch (error) {
        console.error("Failed to get incident shift:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function DeleteIncidentShiftById(id) {
    try {
        showLoader($(".setting"));

        const response = await fetch("/Settings/DeleteIncidentShiftById?id=" + id, {
            method: "GET",
            headers: { "Content-Type": "application/json", "Accept": "text/html" }
        });

        if (!response.ok) throw new Error("Failed to delete incident shift.");

        SwalSuccessAlert("Incident shift deleted successfully!");
        GetAllIncidentShifts();
    } catch (error) {
        console.error("Failed to delete incident shift:", error);
    } finally {
        hideLoader($(".setting"));
    }
}

async function SaveIncidentShift() {
    try {
        let form = [];
        let formData = new FormData();
        let obj = $("#NewIncidentShiftForm")[0];

        let params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });

        showLoader($(".setting"));

        let response = await fetch("/Settings/SaveIncidentShift", {
            method: "POST",
            body: formData
        });

        let result = await response.json();

        if (result.success) {
            $("#addIncidentShift").html("");
            SwalSuccessAlert(result.data);
            GetAllIncidentShifts();
        } else {
            SwalErrorAlert(result.message || "Failed to save incident shift.");
        }
    } catch (error) {
        SwalErrorAlert("Error while saving incident shift!");
        console.error(error);
    } finally {
        hideLoader($(".setting"));
    }
}

function DeleteIncidentShiftItem(id) {
    let confirmBtnText = "Yes, delete it!";
    let cancelBtnText = "No, cancel!";

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.isConfirmed) {
            DeleteIncidentShiftById(id);
        }
    });
}
// End Incident Shift

