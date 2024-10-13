let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros por pagina",
            "zeroRecords": "Ningun registro",
            "info": "Mostrar _PAGE_ de _PAGES_",
            "infoEmpty": "no hay registros",
            "infoFiltered": "(filtered from _MAX_ total registros",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "lasto": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior",
            }
        },

        "ajax": {
            "url": "/Orden/ObtenerListaOrdenes"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "nombreCompleto", "width": "15%" },
            { "data": "telefono", "width": "15%" },
            { "data": "email", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                        <a href="/Orden/Detalle/${data}" class="btn btn-success text-white" style="cursor: pointer;">
                           <i class="fas fa-edit"></i>
                        </a>
                    </div>
                `;
                },
                "width": "5%"
            }
        ]
    });
}