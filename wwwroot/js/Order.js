var dataTable;
$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
   dataTable= $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll",  // Ensure this URL is correct
            "type": "GET"
        },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "10%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "15%" },
            {
                data: 'id',width:"10%",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role=group>
                    <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                    </div>`
                }
            }
          
        ]
    });
}