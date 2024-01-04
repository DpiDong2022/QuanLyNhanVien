
let SelectedIdNhanVien;
const RecordAmount = 10;
let LocPhongBanId = 0;
let TotalPage = 0;
let CurrentPage = 1;
let PhongBanList = [];
let SearchKey = "";

// khởi chaỵ
$(document).ready(function () {
    // lấy danh sách phòng ban
    options = {};
    options.url = "Department/GetList";
    options.type = "get";
    options.textContent = "application/json";

    $.ajax(options).done(function (result) {
        PhongBanList = result;
        const phongBan_selection = $("#phongban-selection");
        const phongban_dropdown = $(".phongban .dropdown-content");

        for (let i = 0; i < PhongBanList.length; i++) {
            var phongBan = PhongBanList[i];

            var dropDownItem = `<a data-id="${phongBan.Id}" onclick="LocPhongBan(${phongBan.Id})">${phongBan.TenPhongBan}</a>`;
            phongban_dropdown.append(dropDownItem);

            var option = `<option data-id="${phongBan.Id}" value="${phongBan.TenPhongBan}">${phongBan.TenPhongBan} </option>`;
            phongBan_selection.append(option);
        }
    });
    renderPageNumber()
});

function changeCurrentPage(number) {
    CurrentPage = number; // number = id
    TimKiem();
}

function ThemNhanVien() {
    SelectedMaNhanVien = ''
    if (LocPhongBanId == 0) return;
    let phongBanTen = PhongBanList.find(c => { return c.Id === LocPhongBanId }).TenPhongBan;
    $("#phongban-selection").val(phongBanTen);
}

function LocPhongBan(Id) {
    LocPhongBanId = Id;
    let phongbanSpan = $(".phongban .dropdown-span");
    if (Id == 0) {
        phongbanSpan.text("--Tất cả--");
    } else {
        let tenPhongBan = PhongBanList.find(c => c.Id == Id).TenPhongBan;
        phongbanSpan.text(tenPhongBan);
    }
    CurrentPage = 1;
    TimKiem();
}

function renderPageNumber() {
    TotalPage = $("#hahaa").val();
    let pagination = document.getElementById("staff_pagination");
    pagination.innerHTML = "";

    for (let index = 1; index <= TotalPage; index++) {
        if(index == CurrentPage){
            pagination.innerHTML += `<button id="${index}" class="btn mx-1"` +
            `onclick="changeCurrentPage(${index})" style="border:2px solid red">$${index}</button>`;
        }else{
            pagination.innerHTML += `<button id="${index}" class="btn mx-1"` +
            `onclick="changeCurrentPage(${index})">$${index}</button>`;
        }
    }
}

function TimKiem(isKeySearchSaved = false) {
    if (isKeySearchSaved) {
        SearchKey = $("#searchInput").val().toLowerCase();
    }
    var options = {};
    options.url = "Staff/Search";
    options.type = "get";
    options.contentType = "application/json";
    options.data = {
        keySearch: SearchKey,
        pageSize: RecordAmount,
        pageNumber: CurrentPage,
        phongBanId: LocPhongBanId
    }

    $.ajax(options).done(function (content) {
        $("#staff-row-container").html(content);
        renderPageNumber();
    });
}

function staff_row(id, hoVaTen, ngaySinh, dienThoai, chucVu, phongBanId) {

    var PhongBan = PhongBanList.find(item => item.Id == phongBanId);

    if (dienThoai == null || dienThoai == "null") {
        dienThoai = "";
    }
    if (chucVu == null || chucVu == "null") {
        chucVu = "";
    }

    return `<tr id="${id}">
                <td>${id}</td>
                <td class="hoten">${hoVaTen}</td>
                <td class="ngaysinh">${DobFormatVN(ngaySinh)}</td>
                <td>${dienThoai}</td>
                <td>${chucVu}</td>
                <td data-id="${PhongBan.Id}">${PhongBan.TenPhongBan}</td>
                <td class="btn-area">
                    <button class="btn bg-warning m-1 edit-staff-btn" data-toggle="modal" 
                        data-target="#container-staff-form" onclick="GiuMaNhanVien(${id})">Sửa</button>
                    <button class="btn text-light bg-danger m-1" data-toggle="modal" 
                        data-target="#removeStaff" onclick="GiuMaNhanVien(${id})">Xóa</button>
                </td>
            </tr>`
}

function fillStaffForm() {
    var name = $("#" + SelectedIdNhanVien).children(".hoten").text();
    var Dob = $("#" + SelectedIdNhanVien).children(".ngaysinh").text();
    var number = $("#" + SelectedIdNhanVien).children("td:nth-child(4)").text();
    var position = $("#" + SelectedIdNhanVien).children("td:nth-child(5)").text();
    var department = $("#" + SelectedIdNhanVien).children("td:nth-child(6)").text();

    $("#HoVaTen").val(name);
    $("#DOB").val(DobFormatEN(Dob));
    $("#DienThoai").val(number);
    $("#ChucVu").val(position);
    $("#phongban-selection").val(department);
}

// validate ngay sinh
function validateDob() {
    const DobInput = $("#DOB").val();
    const currentYear = new Date().getFullYear();
    const yearInput = parseInt(DobInput.substring(0, 4));

    if (DobInput == "") {
        $("#DobError").show();
        $("#DobError").html("*Ngày sinh là bắt buộc");
        $("#error-staff-form").html("");
        return false;
    } else if ((currentYear - yearInput) < 18) {

        $("#DobError").show();
        $("#DobError").html("*Ngày sinh không hợp lệ, nhân viên chưa đủ 18 tuổi");
        $("#error-staff-form").html("");
        return false;
    } else if (isStaffExist()) {
        $("#error-staff-form").html("*Nhân viên đã tồn tại do chùng ngày sinh và họ tên").show();
    } else {
        $("#DobError").hide();
        $("#error-staff-form").hide();
        return true;
    }
}

// validate name
function validateName() {
    const hoTen = $("#HoVaTen").val();
    if (hoTen == "") {
        $("#NameError").show();
        $("#error-staff-form").html("");
        return false;
    } else {
        $("#NameError").hide();
        return true;
    }
}

// validate department
function validateDepartment() {
    const phongBan = $("#phongban-selection option:selected").text();
    if (phongBan == "--Tất cả--") {
        $("#phongBanError").show();
        return false;
    } else {
        $("#phongBanError").hide();
        return true;
    }
}

// clear form thêm/sửa nhân viên
$(document).ready(function () {
    $(".modal").on("hidden.bs.modal", function () {
        document.getElementById("staff-form").reset();
        $("#DobError").hide();
        $("#NameError").hide();
        $("#phongBanError").hide();
        $("#error-staff-form").hide();
    });

});

// kiểm tra dữ liệu thêm/sửa
$(document).ready(function () {
    $("#createBtn").click(function () {
        if (validateDob() & validateName() & validateDepartment()) {
            updateStaff();
        }
    });
})

// Kiem tra thông tin nhan vien ở trong form nhân viên da ton tai
function isStaffExist() {
    let NameInput = $("#HoVaTen").val().toLowerCase();
    let DobInput = DobFormatVN($("#DOB").val());

    var rows = Array.from($("#staff-row-container tr"));
    let name, date, id;
    for (row of rows) {
        name = $(row).children(".hoten").text().toLowerCase();
        date = $(row).children(".ngaysinh").text();
        id = $(row).children("td:nth-child(1)").text();
        if (name == NameInput && DobInput == date && id != SelectedIdNhanVien) {
            return true;
        }
    };
    return false;
}

// Cap Nhat thong tin nhân viên
function updateStaff() {
    let hoTen = $("#HoVaTen").val();
    let Sdt = $("#DienThoai").val();
    let chucVu = $("#ChucVu").val();
    let Dob = $("#DOB").val();

    let PhongBanOption = $("#phongban-selection option:selected");
    let phongBanId = PhongBanOption.attr("data-id");
    var options = {};
    options.url = "Staff/Update";
    options.type = "post";
    options.textContent = "application/json";
    options.data = {
        nhanVien: {
            "HoVaTen": hoTen,
            "NgaySinh": Dob,
            "DienThoai": Sdt,
            "ChucVu": chucVu,
            "PhongBanId": phongBanId
        },
        id: SelectedIdNhanVien
    };

    $(".loading").removeClass("loader--hidden");
    $(".loading").addClass("loader");

    $.ajax(options).done(function (result) {
        debugger
        // loading trouc khi done request/////////////////
        $(".loading").addClass("loader--hidden");
        $(".loader").on("transitionend", function () {
            $(".loading").removeClass("loader");
        });

        $(".close").click();

        if (result == true) {
            var newRow = staff_row(SelectedIdNhanVien, hoTen, Dob, Sdt, chucVu, phongBanId);
            $("#" + SelectedIdNhanVien).replaceWith(newRow);
            $(".staff-table tr#" + SelectedIdNhanVien).css("animation", "leftToRightIn 0.15s linear");
        } else if (result == false) {
            alert("Sua ko thanh cong");
        } else {
            var newRow = staff_row(result, hoTen, Dob, Sdt, chucVu, phongBanId);
            $(".staff-table").children("tbody").prepend(newRow);
            $(".staff-table tr#" + result).css("animation", "leftToRightIn 0.15s linear");
        }
        SelectedIdNhanVien = undefined;
    });
}

// xóa nhân viên
$(document).ready(function () {
    $("#XoaButton").click(function () {
        var options = {};
        options.url = "Staff/Delete?id=" + SelectedIdNhanVien;
        options.type = "delete";
        options.contentType = "application/json";
        options.data = {
            id: SelectedIdNhanVien
        };
        $.ajax(options).done(function () {
            let deletedRow = $("#" + SelectedIdNhanVien);
            deletedRow.css("animation", "leftToRightOut 0.5s linear");

            setTimeout(function () {
                deletedRow.remove()
            }, 500);
            SelectedIdNhanVien = undefined;
        });
    });
});

// lưu mã nhân viên để xóa
function GiuMaNhanVien(id) {
    SelectedIdNhanVien = id;
    fillStaffForm()
}

function ExportToExcel() {
    var options = {};
    options.url = "Staff/Report";
    options.type = "post";
    options.contentType = "application/json";

    $.ajax(options).done(function (filePath) {
        if (filePath = "" & filePath != null) {
            window.location.href = "Staff/Download?filePath=" + filePath + "&fileName=BaoCaoNhanVien.xlsx";
        } else {
            alert("fail to download excel file");
        }
    });
}

// Khởi chạy tìm kiếm khi ấn enter
$(document).ready(function () {
    $("#searchInput").on("keyup", function (event) {
        if (event.key === "Enter") {
            CurrentPage = 1;
            TimKiem(true);
        }
    });
});

function DobFormatEN(dateTime) {
    var ngay = dateTime.substring(0, 2);
    var thang = dateTime.substring(3, 5);
    var nam = dateTime.substring(6, 11);
    return nam + "-" + thang + "-" + ngay;
}

function DobFormatVN(dateTime) {

    var ngay = dateTime.substring(8, 11);
    var thang = dateTime.substring(5, 7);
    var nam = dateTime.substring(0, 4);
    return ngay + "-" + thang + "-" + nam;
}