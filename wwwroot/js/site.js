
let SelectedIdNhanVien;
const RecordAmount = 10;
let CurrentPage = 1;
let LocPhongBanId = 0;
let TotalPage = 0;
let DanhSachNhanVien = [];
let ShowedStaff = [];
let SearchStaff = [];
let PhongBanList = [];

// lấy danh sach nhan vien và phòng ban
$(document).ready(function () {

    // lấy dữ liệu và render phòng ban element
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

    // lấy dữ liệu và render bảng Nhân viên
    options = {};
    options.url = "Staff/List";
    options.type = "get";
    options.textContent = "application/json";

    $.ajax(options).done(function (result) {
        DanhSachNhanVien = result;
        CurrentPage = 1
        TimKiem();
    });
});

function changeCurrentPage(number) {
    CurrentPage = number;
    TimKiem();
}

function LocPhongBan(Id) {
    LocPhongBanId = Id;
    let phongbanSpanP = $(".phongban .dropdown-span p");
    if (Id == 0) {
        phongbanSpanP.html("Phòng ban: --Tất cả--");
    } else {
        let tenPhongBan = PhongBanList.find(c => c.Id == Id).TenPhongBan;
        phongbanSpanP.html("Phòng ban: " + tenPhongBan);
    }
    CurrentPage = 1;
    TimKiem();
}

function renderPageNumber() {
    document.getElementById("staff_pagination").innerHTML = "" + `<button class="btn mx-1"` +
        `onclick="changeCurrentPage(${1})">${1}</button>`;

    for (let index = 2; index <= TotalPage; index++) {
        document.getElementById("staff_pagination").innerHTML += `<button class="btn mx-1"` +
            `onclick="changeCurrentPage(${index})">${index}</button>`;
    }
}

function render_staffs() {
    const table = $("#staff-row-container");
    table.empty();
    let row;

    for (nhanVien of ShowedStaff) {
        row = staff_row(nhanVien.Id, nhanVien.HoVaTen, nhanVien.NgaySinh, nhanVien.DienThoai, nhanVien.ChucVu, nhanVien.PhongBanId);
        table.append(row);
    }
    // dien thong tin tuong ung vao form khi an edit
    $(".edit-staff-btn").click(fillStaffForm);
}

function TimKiem() {
    const searhString = $("#searchInput").val().toLowerCase();

    if (searhString == "" || searhString == null) {
        SearchStaff = DanhSachNhanVien;
    } else {
        const keySplited = searhString.split(" ");
        SearchStaff = DanhSachNhanVien.filter(nhanVien => {
            return keySplited.some(c => nhanVien.HoVaTen.toLowerCase().includes(c))
        });
    }
    // lọc phòng ban
    if (LocPhongBanId != 0) {
        SearchStaff = SearchStaff.filter(nhanVien => {
            return nhanVien.PhongBanId === LocPhongBanId;
        });
    }

    TotalPage = Math.ceil(SearchStaff.length / RecordAmount);
    ShowedStaff = SearchStaff.slice(
        (CurrentPage - 1) * RecordAmount,
        (CurrentPage - 1) * RecordAmount + RecordAmount);

    // render bảng nhân nhiên, nút phân trang
    render_staffs();
    renderPageNumber();
}

function staff_row(id, hoVaTen, ngaySinh, dienThoai, chucVu, phongBanId) {
    var tenPhongBan;

    if (phongBanId == 0 || phongBanId == undefined) {
        tenPhongBan = "Chưa chọn";
    }
    else if (phongBanId != undefined && phongBanId != "") {

        let phongBan = PhongBanList.find(item => item.Id == phongBanId);
        tenPhongBan = phongBan.TenPhongBan;
        phongBanId = phongBan.Id;
    }
    if (dienThoai == null || dienThoai == "null") {
        dienThoai = "";
    }
    if (chucVu == null || chucVu == "null") {
        chucVu = "";
    }
    debugger;

    return `<tr id="${id}">
                <td>${id}</td>
                <td class="hoten">${hoVaTen}</td>
                <td class="ngaysinh">${DobFormat(ngaySinh)}</td>
                <td>${dienThoai}</td>
                <td>${chucVu}</td>
                <td data-id="${phongBanId}">${tenPhongBan}</td>
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
    $("#DOB").val(Dob);
    $("#DienThoai").val(number);
    $("#ChucVu").val(position);

    if (department != "Chưa chọn") {
        $("#phongban-selection").val(department);
    }

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
    } else {
        $("#DobError").hide();
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

// clear form thêm/sửa nhân viên
$(document).ready(function () {
    $(".modal").on("hidden.bs.modal", function () {
        document.getElementById("staff-form").reset();
        $("#DobError").hide();
        $("#NameError").hide();
        $("#error-staff-form").hide();
    });

});

// kiểm tra dữ liệu thêm/sửa
$(document).ready(function () {
    $("#createBtn").click(function () {
        if (validateDob() & validateName()) {

            if (isStaffExist()) {

                $("#error-staff-form").html("*Nhân viên đã tồn tại do chùng ngày sinh và họ tên");
                $("#error-staff-form").show();
            } else {
                $("#error-staff-form").hide();
                updateStaff();
            }
        }
    });
})

// Kiem tra thông tin nhan vien ở trong form nhân viên da ton tai
function isStaffExist() {
    let NameInput = $("#HoVaTen").val().toLowerCase();
    let DobInput = $("#DOB").val();

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

    $.ajax(options).done(function (result) {
        if (result.Success) {
            $(".close").click();
            $(".loading").removeClass("loader--hidden");
            $(".loading").addClass("loader");

            setTimeout(() => {
                $(".loading").addClass("loader--hidden");
            }, 500);

            $(".loader").on("transitionend", function () {
                $(".loading").removeClass("loader");
            });

            setTimeout(() => {
                id = result.data == null ? SelectedIdNhanVien : result.data;
                var newRow = staff_row(id, hoTen, Dob, Sdt, chucVu, phongBanId);
                let nhanVien = {
                    "Id": id,
                    "HoVaTen": hoTen,
                    "NgaySinh": Dob,
                    "DienThoai": Sdt,
                    "ChucVu": chucVu,
                    "PhongBanId": phongBanId
                };
                if (SelectedIdNhanVien == undefined) {

                    $(".staff-table").children("tbody").prepend(newRow);
                    DanhSachNhanVien.push(nhanVien);
                } else {
                    $("#" + id).replaceWith(newRow);
                    index = DanhSachNhanVien.findIndex(function (nhanVien) {
                        return nhanVien.Id === id
                    });
                    DanhSachNhanVien[index] = nhanVien;
                }
                $(".staff-table tr#" + id).css("animation", "leftToRightIn 0.3s linear");
                $(".edit-staff-btn").click(fillStaffForm);
            }, 600);

        } else {
            alert(result.Message);
        }
    });
}

// xóa nhân viên
$(document).ready(function () {
    $("#XoaButton").click(function () {
        var options = {};
        options.url = "Staff/Delete";
        options.type = "get";
        options.contentType = "application/json";
        options.data = {
            id: SelectedIdNhanVien
        };

        $.ajax(options).done(function () {
            index = DanhSachNhanVien.findIndex(c => c.id === SelectedIdNhanVien)
            DanhSachNhanVien.splice(index, 1);

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
}

function ExportToExcel() {
    let nhanVienss = [];

    for (let nv of SearchStaff) {
        var phongban = PhongBanList.find(c => c.Id == nv.PhongBanId);
        if (nv.ChucVu == "null" || nv.ChucVu == null) {
            nv.ChucVu = "";
        }
        if (nv.DienThoai == "null" || nv.DienThoai == null) {
            nv.DienThoai = "";
        }
        var nhanVien = {
            Id: nv.Id,
            HoVaTen: nv.HoVaTen,
            NgaySinh: nv.NgaySinh,
            DienThoai: nv.DienThoai,
            ChucVu: nv.ChucVu,
            PhongBan: {
                Id: 0,
                TenPhongBan: phongban == undefined ? "Chưa Chọn" : phongban.TenPhongBan
            }
        };
        nhanVienss.push(nhanVien);
    };
    var options = {};
    options.url = "Staff/Report";
    options.type = "post";
    options.contentType = "application/json";
    options.data = JSON.stringify(nhanVienss);

    $.ajax(options).done(function (result) {
        if (result.Success) {
            window.location.href = "Staff/Download?filePath=" + result.data + "&fileName=BaoCaoNhanVien.xlsx";
        } else {
            alert(result.Message);
        }
    });
}

// Khởi chạy tìm kiếm khi ấn enter
$(document).ready(function () {
    $("#searchInput").on("keyup", function (event) {
        if (event.key === "Enter") {
            CurrentPage = 1;
            TimKiem();
        }
    });
});

function DobFormat(dateTime) {
    return String(dateTime).substring(0, 10);
}