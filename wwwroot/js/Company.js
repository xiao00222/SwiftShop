var dataTable;
$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
   dataTable= $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll",  // Ensure this URL is correct
            "type": "GET"
        },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'streetAdress', "width": "15%" },
            { data: 'city', "width": "20%" },
            { data: 'state', "width": "10%" },
            { data: 'phoneNumber', "width": "15%" },
            {
                data: 'id',width:"10%",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role=group>
                    <a href="company/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>Edit</a>
                    <a onClick=Delete('company/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i>Delete</a>
                    </div>`
                }
            }
          
        ]
    });
}
function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message)
                }
            })
        }
    });
}