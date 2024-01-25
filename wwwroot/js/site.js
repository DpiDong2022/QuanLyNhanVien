let SelectedPageNumber = 1;

$(document).ready(() => {
    // open staff form to insert
    $(document).on("click", "#create-staff-btn", () => {
        clean_the_form();
        $("#staff-form").attr("data-mode", "Create");
        $(".staff-form-header").text("Thêm nhân viên")
        $("#phongban-selection-form").val($("#phongban-filter").val());
    })

    // listener validate before send request
    $(document).on("submit", "#staff-form", (event) => {
        event.preventDefault()
        // hide the success message
        $(".modal-message").text("");
        if (Validate() == true) {
            create_or_update();
        }
    });

    // prepare the form to edit when the edit btn's clicked
    $(document).on("click", ".edit-staff-btn", function () {
        clean_the_form();
        $("#staff-form").attr("data-mode", "Update");
        $(".staff-form-header").text("Sửa nhân viên");
        Pre_populate(this);
    });

    // delete click
    $(document).on("click", ".delete-staff-btn", function () {
        let id = $(this).attr("data-id")
        $("#modal-remove-staff").attr("data-staff-id", id);
    })

    // delete confirm to delete staff
    $(document).on("click", "#submit-remove-staff", function () {
        let staff_id = $(this).closest("#modal-remove-staff").attr("data-staff-id");
        Delete(staff_id);
    });

    // call search when press enter while search input was focusing
    $("#search-input").on("keyup", function (event) {
        if (event.key == "Enter") {
            SelectedPageNumber = 1;
            $("#phongban-filter").val(-1);
            Search();
        }
    });

    // change department
    $(document).on("change", "#phongban-filter", function () {
        SelectedPageNumber = 1;
        Search();
    })

    // change page number
    $(document).on("click", "#staff-pagination button", function(){
        SelectedPageNumber = $(this).attr("id");
        Search();
    })

    // call search fucntion when search button was clicked
    $("#search-button").click(function () {
        SelectedPageNumber = 1;
        $("#phongban-filter").val(-1);
        Search();
    });

    // rend the page number at the beginning of the program
    rend_page_number();

    // 
    $(".warning-popup button").on("click", function(){
        $(".warning-popup").hide();
    })
});

function DisplayError(actionName, errorResponse){
    $(".action-discription").text("Thực hiện "+ actionName +" không thành công");
    if(errorResponse.Code == 500){
        $(".warning-popup").show();
    }else if(errorResponse.Code == 400){
        $(".reason").text("Do "+errorResponse.Message);
        $(".warning-popup").show();
    }
}

function Search() {
    // get department id
    // get pagination number
    // get key search
    let _phongBanId = $("#phongban-filter").val();
    let key = $("#search-input").val().toLowerCase();
    let _pageSize = 8;
    $.ajax({
        url: "Staff/Search",
        type: "get",
        contentType: "application/json",
        data: {
            keySearch: key,
            phongBanId: _phongBanId,
            PageSize: _pageSize,
            PageNumber: SelectedPageNumber
        },
        success: function (response) {
            if(response.Code == 200){
                $("#staff-container").html(response.Data);
                rend_page_number();
            }else{
                DisplayError("Tìm Kiếm", response)
            }
        }
    });
}

function Delete(staff_id) {
    var options = {};
    options.url = "Staff/Delete?id=" + staff_id;
    options.type = "delete";
    options.contentType = "application/json";
    $.ajax(options).done(function (response) {
        debugger
        if (response.Code == 200) {
            let deletedRow_element = $("#" + staff_id);
            deletedRow_element.css("animation", "leftToRightOut 0.5s linear");
            setTimeout(function () {
                deletedRow_element.remove()
            }, 500);

        } else {
            DisplayError("Xóa", response);
        }
    });
}

function create_or_update() {
    // get form
    let staff_form = document.getElementById("staff-form");
    // display loading
    $(".loading").removeClass("loader--hidden");
    $(".loading").addClass("loader");

    //auto clode the loading if the request do not success
    setTimeout(() => {
        $(".loading").addClass("loader--hidden");
        $(".loading").removeClass("loader");
    }, 3000);

    let mode = staff_form.getAttribute("data-mode");

    $.ajax({
        url: "Staff/" + mode,
        type: "POST",
        data: new FormData(staff_form),
        processData: false,
        contentType: false,
        success: function (response) {
            debugger;
            // close the loading after sending request successed!
            $(".loading").addClass("loader--hidden");
            $(".loading").removeClass("loader");

            if (response.Code == 200) {
                if (mode == "Create") {
                    $(".modal-message").text("Thêm thông tin thành công");
                    Rend_New_Staff_Row(response.Data);
                } else if (mode == "Update") {
                    $(".modal-message").text("Sửa thông tin thành công");
                    Update_Existed_Staff_Row(response.Data);
                }
            }  else if (response.Code == 400) {
                $(".partial-create").html(response.Data);
                $("#staff-form").attr("data-mode", mode);
            } else{
                DisplayError(mode=="Create" ? "Thêm":"Sửa", response)
            }
        },
        error: function () {
            alert("fail")
        }
    });
}

// export data into staff excel template
function ExportToExcel() {
    let _phongBanId = $("#phongban-filter").val();
    let key = $("#search-input").val().toLowerCase();
    debugger
    var options = {};
    options.url = "Staff/Report?";
    options.type = "get";
    options.contentType = "application/json";
    options.data = {
        keySearch : key,
        phongBanId: _phongBanId
    };

    $.ajax(options).done(function (response) {
        if(response.Code == 200){
            window.location.href = "Staff/Download?filePath=" + response.Data + "&fileName=BaoCaoNhanVien.xlsx";
        }else {
            DisplayError("Xuất File Excel", response);
        }
    });
}

function Pre_populate(root) {
    let closest_row_tag = $(root).closest("tr");
    let id = $(closest_row_tag).attr("id");
    let hoVaTen = $(closest_row_tag).find(".hoVaTen").text();
    let ngaySinh = DobFormatEN($(closest_row_tag).find(".ngaySinh").text());
    let chucVu = $(closest_row_tag).find(".chucVu").text();
    let dienThoai = $(closest_row_tag).find(".dienThoai").text();
    let phongBanId = $(closest_row_tag).find(".phongBan").data("id");

    let staff_form = $("#staff-form");
    staff_form.find("#Id").val(id)
    staff_form.find("#HoVaTen").val(hoVaTen);
    staff_form.find("#NgaySinh").val(ngaySinh);
    staff_form.find("#ChucVu").val(chucVu);
    staff_form.find("#DienThoai").val(dienThoai);
    staff_form.find("#phongban-selection-form").val(phongBanId);
}

function clean_the_form() {
    $(".modal-message").text("");
    $("#staff-form input:not([type='date'])").val("");
    $(".error-holder").text("");
}

// rend pagination
function rend_page_number() {
    totalPage = $("#hidden-totalpage").val();
    let pagination = document.getElementById("staff-pagination");
    pagination.innerHTML = "";

    for (let index = 1; index <= totalPage; index++) {
        if (index == SelectedPageNumber) {
            pagination.innerHTML += `<button id="${index}" class="btn mx-1"` +
                `onclick="changeCurrentPage(${index})" style="border:2px solid red; background-color:white">${index}</button>`;
        } else {
            pagination.innerHTML += `<button id="${index}" class="btn mx-1"` +
                `>${index}</button>`;
        }
    }
}

// rend new staff row
function get_staff_row(staffObj) {

    return `<tr id="${staffObj.Id}">
                <td class="id">${staffObj.Id}</td>
                <td class="hoVaTen">${staffObj.HoVaTen}</td>
                <td class="ngaySinh">${DobFormatVN(staffObj.NgaySinh)}</td>
                <td class="dienThoai">${staffObj.DienThoai}</td>
                <td class="chucVu">${staffObj.ChucVu}</td>
                <td class="phongBan" data-id="${staffObj.PhongBanId}">${staffObj.PhongBan.TenPhongBan}</td>
                <td class="btn-area">
                    <button data-id=${staffObj.Id} class="btn bg-warning m-1 edit-staff-btn" data-toggle="modal" 
                        data-target="#modal-edit-staff">Sửa</button>
                    <button data-id=${staffObj.Id} class="btn text-light bg-danger m-1 delete-staff-btn" data-toggle="modal" 
                        data-target="#modal-remove-staff">Xóa</button>
                </td>
            </tr>`
}

// rend the new tr staff row
function Rend_New_Staff_Row(staffObj) {
    $("#staff-container").prepend(get_staff_row(staffObj));
    $("#staff-container tr#" + staffObj.Id).css("animation", "leftToRightIn 0.15s linear");
}
// update the changed tr staff row
function Update_Existed_Staff_Row(staffObj) {
    $("#staff-container #" + staffObj.Id).replaceWith(get_staff_row(staffObj));
    $(".staff-table tr#" + staffObj.Id).css("animation", "leftToRightIn 0.15s linear");
}

function Validate() {
    return (Validate_NgaySinh() & Validate_HoTen() & Validate_PhongBan());
}

// validate ngay sinh
function Validate_NgaySinh() {
    const dobInput = new Date($("#NgaySinh").val());
    const currentDate = new Date();

    if (dobInput >= currentDate) {

        $("#DobError").html("Sinh nhật không được lớn hơn ngày tháng năm hiện tại, hãy điền ngày sinh nhật của bạn").css("color", "red");
        return false;
    } else if (dobInput.getFullYear() <= 1800) {

        $("#DobError").html("Cảnh báo: Ngày sinh nhật quá xa so với ngày hiện tại").css("color", "green");
        return true;
    } else {
        $("#DobError").html("").css("color", "red");
        return true;
    }
}

// validate name
function Validate_HoTen() {
    let hoVaTen = $("#HoVaTen").val();
    if (hoVaTen.length == 0) {
        $("#NameError").html("Họ và tên là yêu cầu");
        return false;
    } else {
        $("#NameError").html("");
        return true;
    }
}

// validate department
function Validate_PhongBan() {
    let phongBan = $("#phongban-selection-form option:selected").text();
    if (phongBan == "--Tất cả--") {
        $("#phongBanError").html("Phòng ban là yêu cầu");
        return false;
    } else {
        $("#phongBanError").html("");
        return true;
    }
}

// fortmat date
function DobFormatEN(dateTime) {
    var ngay = dateTime.substring(0, 2);
    var thang = dateTime.substring(3, 5);
    var nam = dateTime.substring(6, 11);
    return nam + "-" + thang + "-" + ngay;
}

function DobFormatVN(dateTime) {
    var ngay = dateTime.substring(8, 10);
    var thang = dateTime.substring(5, 7);
    var nam = dateTime.substring(0, 4);
    return ngay + "-" + thang + "-" + nam;
}