let SelectedPageNumber = 1;

$(document).ready(() => {
    // add - bắt sự kiện click thêm nhân viên
    $("#create-staff-btn").on("click", () => {
        Clear_the_form();
        $("#staff-form").attr("data-mode", "Create");
        $(".staff-form-header").text("Thêm nhân viên")
        $("#phongban-selection-form").val($("#phongban-filter").val());
        $("#chucvu-selection-form").val($("#chucvu-filter").val());
    });

    // submit - bắt sự kiện submit form nhân viên
    $(document).on("submit", "#staff-form", (event) => {
        event.preventDefault()
        if (Validate() == true) {
            Create_or_update();
        }
    });

    // edit - click - bắt sự kiện click sửa nhân viên
    $(document).on("click", ".edit-staff-btn", function () {
        Clear_the_form();
        $("#staff-form").attr("data-mode", "Update");
        $(".staff-form-header").text("Sửa nhân viên");
        Pre_populate(this);
    });

    // delete - click - bắt sự kiện click xóa nhân viên
    $(document).on("click", ".delete-staff-btn", function () {
        let id = $(this).attr("data-id")
        $("#modal-remove-staff").attr("data-staff-id", id);
    })

    // delete - confirm - bắt sự kiện xác nhận xóa nhân viên
    $("#submit-remove-staff").on("click", function () {
        let staff_id = $(this).closest("#modal-remove-staff").attr("data-staff-id");
        Delete(staff_id);
    });

    // search - bắt sự kiện ấn "ENTER" khi người dùng đang nhập liệu
    $("#search-input").on("keyup", function (event) {
        if (event.key == "Enter") {
            SelectedPageNumber = 1;
            Search();
        }
    });

    // search - bắt sự kiện click tìm kiếm
    $("#search-button").click(function () {
        SelectedPageNumber = 1;
        Search();
        window.scrollTo(0, document.body.scrollHeight);
    });

    //
    $("#search-input").on("emptied", ()=>{
        SelectedPageNumber = 1;
        Search();
    });

    // change - thay đổi phòng ban
    $("#phongban-filter").on("change", function () {
        SelectedPageNumber = 1;
        Search();
    })

    // change - thay đổi chức vụ
    $("#chucvu-filter").on("change", function () {
        SelectedPageNumber = 1;
        Search();
    })

    // change - chuyển trang
    $(document).on("click", "#staff-pagination button", function () {
        SelectedPageNumber = $(this).attr("id");
        Search();
    })

    // rend nút phân trang
    Rend_page_number();

    // click - dismiss bắt sự kiện dismiss thông báo lỗi
    $(".warning-popup button").on("click", ()=> $(".warning-popup").hide());

    // (chọn | bỏ chọn) lọc
    $(".IsFilted").on("click", Search);
});

// hiển thị lỗi 400 | 500
function DisplayError(actionName, errorResponse) {
    $(".action-discription").text("Thực hiện " + actionName + " không thành công");
    if (errorResponse.Code == 500) {
        $(".warning-popup").show();
    } else if (errorResponse.Code == 400) {
        $(".reason").text("Do " + errorResponse.Message);
        $(".warning-popup").show();
    }
}

function Search() {
    let _phongBanId = -1,
        _chucVuId = -1;

    if($(".IsFilted").prop("checked")){
        _phongBanId = $("#phongban-filter").val();
        _chucVuId = $("#chucvu-filter").val();
    }

    let key = $("#search-input").val().trim().toLowerCase();
    $.ajax({
        url: "Staff/Search",
        type: "get",
        contentType: "application/json",
        data: {
            keySearch: key,
            phongBanId: _phongBanId,
            chucVuId: _chucVuId,
            PageNumber: SelectedPageNumber
        },
        success: function (response) {
            if (response.Code == 200) {
                $("#staff-container").html(response.Data);
                Rend_page_number();
            } else {
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
        if (response.Code == 200) {
            
            // trừ 1 cho mỗi stt bên dưới element bị xóa
            var alls = Array.from($("#staff-container tr#"+staff_id).nextAll("tr"));
            alls.forEach((tr) => {
                var sttElement = $(tr).find(".stt");
                sttElement.text(+sttElement.text()-1);
            })

            AddAnimation("#" + staff_id, "leftToRightOut 0.5s linear");
            setTimeout(function () {
                $("#" + staff_id).remove()
            }, 500);
           

        } else {
            DisplayError("Xóa", response);
        }
    });
}

// thêm hoặc sửa nhân viên dựa trên data-mode trên form
function Create_or_update() {  
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
            // tắt loading khi gửi request thành công
            $(".loading").addClass("loader--hidden");
            $(".loading").removeClass("loader");

            if (response.Code == 200) {
                $("#cancel-delete").trigger("click");
                
                if (mode == "Create") {
                    Rend_new_staff_row(response.Data);
                } else if (mode == "Update") {
                    if (response.Data) {
                        Update_existed_staff_row();
                    }
                }
            } else if (response.Code == 400) {
                $(".partial-create").html(response.Data);
                $("#staff-form").attr("data-mode", mode);
            } else {
                DisplayError(mode == "Create" ? "Thêm" : "Sửa", response)
            }
        },
        error: function () {
            alert("Bạn đã mất kết lối với trang chủ")
        }
    });
}

// xuất file excel
function ExportToExcel() {
    let _phongBanId = -1,
        _chucVuId = -1;

    if($(".IsFilted").prop("checked")){
        _phongBanId = $("#phongban-filter").val();
        _chucVuId = $("#chucvu-filter").val();
    }
    let key = $("#search-input").val().toLowerCase();
    var options = {};
    options.url = "Staff/Report?";
    options.type = "get";
    options.contentType = "application/json";
    options.data = {
        keySearch: key,
        phongBanId: _phongBanId,
        chucVuId: _chucVuId
    };

    $.ajax(options).done(function (response) {
        if (response.Code == 200) {
            window.location.href = "Staff/Download?filePath=" + response.Data + "&fileName=BaoCaoNhanVien.xlsx";
        } else {
            DisplayError("Xuất File Excel", response);
        }
    });
}

function Clear_the_form() {
    $("#staff-form input").val("");
    $(".error-holder").text("");
}

// lấy dữ liệu từ form nhân viên
function Get_staff_obj() {
    let staff_model = {};
    let staff_form = $("#staff-form");
    staff_model.Stt =  $(".partial-create").attr("data-stt");
    staff_model.Id = staff_form.find("#Id").val();
    staff_model.HoVaTen = staff_form.find("#HoVaTen").val();
    staff_model.NgaySinh = staff_form.find("#NgaySinh").val();
    staff_model.DienThoai = staff_form.find("#DienThoai").val();

    let phongBan = staff_form.find("#phongban-selection-form option:selected");
    let chucVu = staff_form.find("#chucvu-selection-form option:selected");

    staff_model.PhongBan = {
        Id: phongBan.val(),
        TenPhongBan: phongBan.text()
    }
    staff_model.ChucVu = {
        Id: chucVu.val(),
        TenChucVu: chucVu.text()
    }

    return staff_model;
}

// rend nút phân trang
function Rend_page_number() {
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

function Pre_populate(root) {
    let closest_row_tag = $(root).closest("tr");
    let stt = $(closest_row_tag).find(".stt").text();
    let id = $(closest_row_tag).attr("id");
    let hoVaTen = $(closest_row_tag).find(".hoVaTen").text();
    let ngaySinh = DobFormatEN($(closest_row_tag).find(".ngaySinh").text());
    let dienThoai = $(closest_row_tag).find(".dienThoai").text();
    let phongBanId = $(closest_row_tag).find(".phongBan").data("id");
    let chucVuId = $(closest_row_tag).find(".chucVu").data("id");

    let staff_form = $("#staff-form");
    $(".partial-create").attr("data-stt", stt);
    staff_form.find("#Id").val(id)
    staff_form.find("#HoVaTen").val(hoVaTen);
    staff_form.find("#NgaySinh").val(ngaySinh);
    staff_form.find("#DienThoai").val(dienThoai);
    staff_form.find("#phongban-selection-form").val(phongBanId);
    staff_form.find("#chucvu-selection-form").val(chucVuId);
}

// rend new staff row
function Get_staff_row_html(staffObj) {

    return `<tr id="${staffObj.Id}">
                <td class="stt">${staffObj.Stt}</td>
                <td class="id">${staffObj.Id}</td>
                <td class="hoVaTen">${staffObj.HoVaTen}</td>
                <td class="ngaySinh">${DobFormatVN(staffObj.NgaySinh)}</td>
                <td class="dienThoai">${staffObj.DienThoai}</td>
                <td class="phongBan" data-id="${staffObj.PhongBan.Id}">${staffObj.PhongBan.TenPhongBan}</td>
                <td class="chucVu" data-id="${staffObj.ChucVu.Id}">${staffObj.ChucVu.TenChucVu}</td>
                <td class="btn-area">
                    <button data-id=${staffObj.Id} class="btn bg-warning m-1 edit-staff-btn" data-toggle="modal" 
                        data-target="#modal-edit-staff">Sửa</button>
                    <button data-id=${staffObj.Id} class="btn text-light bg-danger m-1 delete-staff-btn" data-toggle="modal" 
                        data-target="#modal-remove-staff">Xóa</button>
                </td>
            </tr>`
}

// rend the new tr staff row
function Rend_new_staff_row(Id) {
    let newNhanVienObj = Get_staff_obj();
    newNhanVienObj.Id = Id;
    let highest_stt = +$("#staff-container tr:last .stt").text();
    newNhanVienObj.Stt = highest_stt + 1;
    $("#staff-container").append(Get_staff_row_html(newNhanVienObj));

    var selector = "#staff-container tr#" + Id;
    AddAnimation(selector, "leftToRightIn 0.15s linear");
    AddCssTimeOut(selector, "border", "4px solid green", 1000);
    window.scrollTo(0, document.body.scrollHeight);
}

// cập nhật hàng dữ liệu đã chỉnh sửa
function Update_existed_staff_row() {
    let newNhanVienObj = Get_staff_obj();
    var newRow = Get_staff_row_html(newNhanVienObj);
    var selector = "#staff-container tr#" + newNhanVienObj.Id;
    $(selector).replaceWith(newRow);

    AddAnimation(selector, "leftToRightIn 0.15s linear");
    AddCssTimeOut(selector, "border", "4px solid green", 700);
}

function AddAnimation(selector, value) {
    $(selector).css("animation", value);
}

function AddCssTimeOut(selector, attrName, value, miniSecond) {
    $(selector).css(attrName, value);
    setTimeout(() => {
        $(selector).css(attrName, "");
    }, miniSecond);
}

function Validate() {
    return (Validate_NgaySinh() & Validate_HoTen() & Validate_PhongBan() & Validate_ChucVu());
}

// validate ngay sinh
function Validate_NgaySinh() {
    let dobInput;
    let dobInputString = $("#NgaySinh").val();
    const currentDate = new Date();
    if (dobInputString == "") {
        $("#DobError").html("Ngày tháng năm sinh là yêu cầu").css("color", "red");
        return false;
    }
    else {
        dobInput = new Date($("#NgaySinh").val());
        if (dobInput >= currentDate) {

            $("#DobError").html("Sinh nhật không được lớn hơn ngày tháng năm hiện tại, hãy điền ngày sinh nhật của bạn").css("color", "red");
            return false;
        }
    }
    if (dobInput.getFullYear() <= 1800) {

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
    let phongBan = $("#phongban-selection-form option:selected").val();
    if (phongBan == -1) {
        $("#phongBanError").html("Phòng ban là yêu cầu");
        return false;
    } else {
        $("#phongBanError").html("");
        return true;
    }
}

// validate Chuc Vu
function Validate_ChucVu() {
    let chucVu = $("#chucvu-selection-form option:selected").val();
    if (chucVu == -1) {
        $("#chucVuError").html("Chức vụ là yêu cầu");
        return false;
    } else {
        $("#chucVuError").html("");
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