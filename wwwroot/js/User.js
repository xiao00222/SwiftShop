var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll",
            "type": "GET"
        },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'company.name', "width": "10%" },
            { data: 'role', "width": "15%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
            <div class="text-center d-flex justify-content-center gap-2">
                <a onclick=LockUnlock('${data.id}') class="btn btn-danger btn-sm text-white" style="width:100px; cursor:pointer;">
                    <i class="bi bi-lock-fill"></i> Lock
                </a>
                <a class="btn btn-danger btn-sm text-white" style="width:150px; cursor:pointer;">
                    <i class="bi bi-pencil-square"></i> Permission
                </a>
            </div>
        `;
         } else {
                 return `
            <div class="text-center d-flex justify-content-center gap-2">
                <a onclick=LockUnlock('${data.id}')  class="btn btn-success btn-sm text-white" style="width:100px; cursor:pointer;">
                    <i class="bi bi-unlock-fill"></i> Unlock
                </a>
                <a class="btn btn-danger btn-sm text-white" style="width:150px; cursor:pointer;">
                    <i class="bi bi-pencil-square"></i> Permission
                </a>
            </div>
        `;
                    }
                },

                "width": "15%"
            }
        ]
    });
}
function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    })
}
