// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
let SelectedMaNhanVien;

$(document).ready(function(){
    let options = {};
    options.url = "Staff/List";
    options.type = "get";
    options.textContent = "application/json";

    $.ajax(options).done(function(result){
        console.log(result);
    });
});

function validateDob() {
    const dateOfBirth = String($("#DOB").val());
    const currentYear = new Date().getFullYear();
    const userYear = parseInt(dateOfBirth.substring(0, 4));

    if (dateOfBirth == "") {
        $("#DobError").show();
        $("#DobError").html("*Ngày sinh là bắt buộc");
        $("#error-staff-form").html("");
        return false;
    } else if ((currentYear - userYear) < 18) {

        $("#DobError").show();
        $("#DobError").html("*Ngày sinh không hợp lệ, nhân viên chưa đủ 18 tuổi");
        $("#error-staff-form").html("");
        return false;
    } else {
        $("#DobError").hide();
        return true;
    }
}

$(document).ready(function () {
    $(".edit-staff-btn").click(fillStaffForm);
});

function fillStaffForm() {
    var name = $("#" + SelectedMaNhanVien).children(".hoten").text();
    var Dob = String($("#" + SelectedMaNhanVien).children(".ngaysinh").text());
    var number = $("#" + SelectedMaNhanVien).children("td:nth-child(4)").text();
    var position = $("#" + SelectedMaNhanVien).children("td:nth-child(5)").text();

    let firstDashIndex = Dob.indexOf("-");
    let lastDashIndex = Dob.lastIndexOf("-");
    let day = ("0" + Dob.substring(0, firstDashIndex)).slice(-2);
    let month = ("0" + Dob.substring(firstDashIndex + 1, lastDashIndex)).slice(-2);
    let year = Dob.substring(lastDashIndex + 1);

    $("#HoVaTen").val(name);
    $("#DOB").val(year + "-" + month + "-" + day);
    $("#SoDienThoai").val(number);
    $("#ChucVu").val(position);
}

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

$(document).ready(function () {
    $(".modal").on("hidden.bs.modal", function () {
        document.getElementById("staff-form").reset();
        $("#NameError").hide();
        $("#error-staff-form").hide();
        $("#DobError").hide();
    });

});

$(document).ready(function () {
    $("#createBtn").click(function () {
        if (validateDob() & validateName()) {

            if (isStaffExist()) {

                $("#error-staff-form").html("*Nhân viên đã tồn tại do chùng ngày sinh và họ tên");
                $("#error-staff-form").show();
            } else {
                $("#error-staff-form").hide();
                callCreateStaff();
            }
        }
    });
})

function isStaffExist() {
    let currentName = $("#HoVaTen").val().toLowerCase();
    let currentDob = $("#DOB").val();

    let firstDashIndex = currentDob.indexOf("-");
    let lastDashIndex = currentDob.lastIndexOf("-");
    let day = ("0" + currentDob.substring(lastDashIndex + 1)).slice(-2);
    let month = ("0" + currentDob.substring(firstDashIndex + 1, lastDashIndex)).slice(-2);
    let year = currentDob.substring(0, 4);

    let DobFormated = day + "-" + month + "-" + year;

    var rows = document.querySelectorAll(".data");
    let name, date, maNV;
    for (row of rows) {
        name = $(row).children(".hoten").text().toLowerCase();
        date = $(row).children(".ngaysinh").text();
        maNV = $(row).children("td:nth-child(1)").text();
        if (name == currentName && DobFormated == date && maNV != SelectedMaNhanVien) {
            return true;
        }
    };
    return false;
}

// Thêm nhân viên
function callCreateStaff() {
    let hoTen = $("#HoVaTen").val();
    let Sdt = $("#SoDienThoai").val();
    let chucVu = $("#ChucVu").val();

    let Dob = $("#DOB").val();
    let firstDashIndex = Dob.indexOf("-");
    let lastDashIndex = Dob.lastIndexOf("-");
    let day = Dob.substring(lastDashIndex + 1);
    let month = Dob.substring(firstDashIndex + 1, lastDashIndex);
    let year = Dob.substring(0, 4);

    let DobFormated = day + "-" + month + "-" + year;

    var options = {};
    options.url = "Staff/Update";
    options.type = "post";
    options.textContent = "application/json";
    options.data = {
        "MaNhanVien": SelectedMaNhanVien,
        "HoVaTen": hoTen,
        "NgayThangNamSinh": Dob,
        "SoDienThoai": Sdt,
        "ChucVu": chucVu
    };

    $.ajax(options).done(function (result) {
        if (result.success) {
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
                var newRow = "<tr class='data' id='" + result.data + "'>" +
                    "<td>" + result.data + "</td>" +
                    "<td class='hoten'>" + hoTen + "</td>" +
                    "<td class='ngaysinh'>" + DobFormated + "</td>" +
                    "<td>" + Sdt + "</td>" +
                    "<td>" + chucVu + "</td>" +
                    "<td class='btn-area'>" +
                    "<button class ='btn text-light bg-danger m-1 edit-staff-btn' data-toggle='modal' data-target='#container-staff-form'" +
                    "onclick='GiuMaNhanVien(`" + result.data + "`)'>Sửa</button>" +
                    "<button class='btn bg-warning m-1' data-toggle='modal' data-target='#removeStaff'" +
                    "onclick='GiuMaNhanVien(`" + result.data + "`)'>Xóa</button>" +
                    "</td>" +
                    "</tr>";

                if (SelectedMaNhanVien === "" || SelectedMaNhanVien === null) {
                    console.log("1")
                    $(".staff-table").children("tbody").prepend(newRow);
                } else {
                    console.log("2")
                    $("#" + SelectedMaNhanVien).replaceWith(newRow);
                }
                $(".edit-staff-btn").click(fillStaffForm);
            }, 600);

        } else {
            alert("Nhan Vien da ton tai");
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
            MaNhanVien: SelectedMaNhanVien
        };

        $.ajax(options).done(function () {
            let dongBiXoa = $("#" + SelectedMaNhanVien);

            dongBiXoa.addClass("bg-danger");
            dongBiXoa.animate(600, {
                right: "100px"
            })

            setTimeout(function () {
                dongBiXoa.remove()
            }, 400);
        });
    });
});

// lưu mã nhân viên để xóa
function GiuMaNhanVien(MaNhanVien) {
    SelectedMaNhanVien = MaNhanVien;
}

// Khởi chạy tìm kiếm khi ấn enter
$(document).ready(function () {
    $("#searchInput").on("keyup", function (event) {
        if (event.key === "Enter") {
            /*$("#searchButton").click();*/
            TimKiem();
        }
    });
});

function TimKiem() {
    var searhString = $("#searchInput").val().toLowerCase();
    var keySplited = searhString.split(" ");
    const tableRows = document.getElementsByClassName("data");
    const rows = Array.from(tableRows);

    var hoTen;
    rows.forEach(row => {
        hoTen = $(row).find(".hoten").text().toLowerCase();

        if (keySplited.some((c) => hoTen.includes(c))) {

            $(row).show(200);
        } else {
            $(row).hide(200);
        }
    });
}




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