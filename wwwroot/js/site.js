// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
let SelectedIdNhanVien;
let DanhSachNhanVien = [];
let currentPage = 1;
const recordAmount = 10;
let locPhongBanId = 0;
let totalPage = 0;
let showedStaff = [];
let searchStaff = [];
let phongBanList = [];


// tai len danh sach nhan vien
$(document).ready(function () {

    let options2 = {};
    options2.url = "Department/GetList";
    options2.type = "get";
    options2.textContent = "application/json";

    $.ajax(options2).done(function (result) {
        phongBanList = result;

        const departmentTable = $("#department-body");
        const phongBan_selection = $("#phongban-selection");
        const phongban_dropdown = $(".phongban .dropdown-content");

        for (let i = 0; i < phongBanList.length; i++) {
            var Pb = phongBanList[i];
            var row = `<tr id="${Pb.Id}">
                              <td>${Pb.Id}</td>
                                <td>${Pb.TenPhongBan}</td>
                            <td>
                                <button class="btn bg-light">Edit</button>
                                <button class="btn btn-danger">Delete</button>
                            </td>
                        </tr>`;

            var dropDownItem = `<a data-id="${Pb.Id}" onclick="LocPhongBan(${Pb.Id})">${Pb.TenPhongBan}</a>`;
            phongban_dropdown.append(dropDownItem);

            departmentTable.append(row);

            var option = `<option data-id="${Pb.Id}" value="${Pb.TenPhongBan}">
                ${Pb.TenPhongBan}
            </option>`;
            phongBan_selection.append(option);
        }
    });

    // render nhan vien
    let options = {};
    options.url = "Staff/List";
    options.type = "get";
    options.textContent = "application/json";

    $.ajax(options).done(function (result) {
        DanhSachNhanVien = result;
        currentPage = 1
        TimKiem();
    });
});

function changeCurrentPage(number) {
    currentPage = number;
    TimKiem();
}

function LocPhongBan(Id) {
    locPhongBanId = Id;
    let phongbanSpan = $(".phongban .dropdown-span");
    if (Id == 0) {
        phongbanSpan.text("--Tất cả--");
    } else {
        let tenPhongBan = phongBanList.find(c => c.Id == Id).TenPhongBan;
        phongbanSpan.text(tenPhongBan);
    }

    TimKiem(true);
}

function renderPageNumber() {
    document.getElementById("staff_pagination").innerHTML = "";
    for (let index = 1; index <= totalPage; index++) {
        document.getElementById("staff_pagination").innerHTML += `<button class="btn bg-light mx-1"` +
            `onclick="changeCurrentPage(${index})">${index}</button>`;
    }
}

function render_staffs() {
    const table = $("#staff-row-container");
    table.empty();
    let row;

    for (nhanVien of showedStaff) {
        row = staff_row(nhanVien.Id, nhanVien.HoVaTen, nhanVien.NgaySinh, nhanVien.DienThoai, nhanVien.ChucVu, nhanVien.PhongBanId);
        table.append(row);
    }
    // dien thong tin tuong ung vao form khi an edit
    $(".edit-staff-btn").click(fillStaffForm);
}

function TimKiem(isFromBtn = false) {
    if (isFromBtn) currentPage = 1;

    const searhString = $("#searchInput").val().toLowerCase();
    const keySplited = searhString.split(" ");

    if (searhString == "" || searhString == null) {
        if (locPhongBanId == 0) {
            searchStaff = DanhSachNhanVien;
            showedStaff = DanhSachNhanVien.slice(
                (currentPage - 1) * recordAmount,
                (currentPage - 1) * recordAmount + recordAmount);
            totalPage = Math.ceil(DanhSachNhanVien.length / recordAmount);
        } else {
            showedStaff = DanhSachNhanVien.filter(nhanVien => {
                return nhanVien.PhongBanId == locPhongBanId;
            });
            searchStaff = showedStaff;
            totalPage = Math.ceil(showedStaff.length / recordAmount);
            showedStaff = showedStaff.slice(
                (currentPage - 1) * recordAmount,
                (currentPage - 1) * recordAmount + recordAmount);
        }
    } else {
        // lọc ra dòng cần tìm
        if (locPhongBanId == 0) {
            showedStaff = DanhSachNhanVien.filter(nhanVien => {
                return keySplited.some(c => nhanVien.HoVaTen.toLowerCase().includes(c))
            });
        } else {
            showedStaff = DanhSachNhanVien.filter(nhanVien => {
                return (keySplited.some(c => nhanVien.HoVaTen.toLowerCase().includes(c))
                    && nhanVien.PhongBanId == locPhongBanId)
            });
        }
        searchStaff = showedStaff;
        totalPage = Math.ceil(showedStaff.length / recordAmount);
        showedStaff = showedStaff.slice(
            (currentPage - 1) * recordAmount,
            (currentPage - 1) * recordAmount + recordAmount);
    }


    // render
    render_staffs();
    renderPageNumber();
}

function staff_row(id, hoVaTen, ngaySinh, dienThoai, chucVu, phongBanId) {
    var ten = "Chưa chọn";

    if (phongBanId != undefined && phongBanId != "") {

        let phongBan = phongBanList.find(item => item.Id == phongBanId);
        ten = phongBan.TenPhongBan;
        phongBanId = phongBan.Id;
    }
    debugger;
    if (dienThoai == null || dienThoai == "null") {
        dienThoai = "";
    }
    if (chucVu == null || chucVu == "null") {
        chucVu = "";
    }

    return `<tr id="${id}">
                <td>${id}</td>
                <td class="hoten">${hoVaTen}</td>
                <td class="ngaysinh">${DobFormat(ngaySinh)}</td>
                <td>${dienThoai}</td>
                <td>${chucVu}</td>
                <td data-id="${phongBanId}">${ten}</td>
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
    var Department = $("#" + SelectedIdNhanVien).children("td:nth-child(6)").text();

    $("#HoVaTen").val(name);
    $("#DOB").val(Dob);
    $("#DienThoai").val(number);
    $("#ChucVu").val(position);

    if (Department != "Chưa chọn") {
        $("#phongban-selection").val(Department);
    }

}

// validate ngay sinh
function validateDob() {
    const dateOfBirth = $("#DOB").val();
    const currentYear = new Date().getFullYear();
    const inputYear = parseInt(dateOfBirth.substring(0, 4));

    if (dateOfBirth == "") {
        $("#DobError").show();
        $("#DobError").html("*Ngày sinh là bắt buộc");
        $("#error-staff-form").html("");
        return false;
    } else if ((currentYear - inputYear) < 18) {

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

// make defautl for staff form
$(document).ready(function () {
    $(".modal").on("hidden.bs.modal", function () {
        document.getElementById("staff-form").reset();
        $("#DobError").hide();
        $("#NameError").hide();
        $("#error-staff-form").hide();
    });

});

// event nhan nut tao nhan vien
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

// Kiem tra nhan vien da ton tai
function isStaffExist() {
    let currentName = $("#HoVaTen").val().toLowerCase();
    let currentDob = $("#DOB").val();

    var rows = Array.from($("#staff-row-container tr"));
    let name, date, id;
    for (row of rows) {
        name = $(row).children(".hoten").text().toLowerCase();
        date = $(row).children(".ngaysinh").text();
        id = $(row).children("td:nth-child(1)").text();
        if (name == currentName && currentDob == date && id != SelectedIdNhanVien) {
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

            //
            $(".loader").on("transitionend", function () {
                $(".loading").removeClass("loader");
            });

            setTimeout(() => {
                id = result.data == null ? SelectedIdNhanVien : result.data;
                var newRow = staff_row(id, hoTen, Dob, Sdt, chucVu, phongBanId);

                if (SelectedIdNhanVien == undefined) {

                    $(".staff-table").children("tbody").prepend(newRow);
                    DanhSachNhanVien.push({
                        "Id": id,
                        "HoVaTen": hoTen,
                        "NgaySinh": Dob,
                        "DienThoai": Sdt,
                        "ChucVu": chucVu,
                        "PhongBanId": phongBanId
                    });
                    debugger;
                    console.log(DanhSachNhanVien);
                } else {
                    $("#" + id).replaceWith(newRow);
                    index = DanhSachNhanVien.findIndex(function (nhanVien) {
                        return nhanVien.Id === id
                    });
                    DanhSachNhanVien[index] = {
                        "Id": id,
                        "HoVaTen": hoTen,
                        "NgaySinh": Dob,
                        "DienThoai": Sdt,
                        "ChucVu": chucVu,
                        "PhongBanId": phongBanId
                    }
                }
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

            deletedRow.addClass("bg-danger");
            deletedRow.animate(600, {
                right: "100px"
            })

            setTimeout(function () {
                deletedRow.remove()
            }, 400);
        });
    });
});

// lưu mã nhân viên để xóa
function GiuMaNhanVien(id) {
    SelectedIdNhanVien = id;
}

function ExportToExcel() {
    let nhanVienss = [];

    for (let nv of searchStaff) {
        var phongban = phongBanList.find(c => c.Id == nv.PhongBanId);
        if(nv.ChucVu =="null" || nv.ChucVu == null){    
            nv.ChucVu = "";
        }
        if(nv.DienThoai =="null" || nv.DienThoai == null){    
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
    debugger;
    var options = {};
    options.url = "Staff/Report";
    options.type = "post";
    options.contentType = "application/json";
    options.data = JSON.stringify(nhanVienss);

    $.ajax(options).done(function (data) {
        window.location.href = "Staff/Download?filePath=" + data + "&fileName=BaoCaoNhanVien.xlsx";
    });
}

// Khởi chạy tìm kiếm khi ấn enter
$(document).ready(function () {
    currentPage = 1;
    $("#searchInput").on("keyup", function (event) {
        if (event.key === "Enter") {
            TimKiem(true);
        }
    });
});

// Login
let index = 0
function OpenStaff() {
    const password = $("#pass").val();
    const confirm_password = $("#cpass").val();

    if (password == "" || confirm_password == "") return

    if (password == confirm_password) {
        window.open("Index", "_blank")
    }
}

$(document).ready(function () {
    $("#createBtn").click(function () {
        if ($("#container-staff-form").attr("display") == "none") {

        }
    })
});

function DobFormat(dateTime) {
    return String(dateTime).substring(0, 10);
}