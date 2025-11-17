$(document).ready(function () {
    let isEditMode = false;
    let currentSalaId = null;

    // Abrir modal para crear
    $('#btnNuevaSala').on('click', function () {
        isEditMode = false;
        currentSalaId = null;
        $('#modalTitle').text('Crear Nueva Sala');
        $('#modalSubtitle').text('Ingresa la información de la nueva sala de sistemas');
        $('#salaForm')[0].reset();
        $('#salaId').val('');
        $('#Estado').val('1');
        $('#salaModal').fadeIn(300);
    });

    // Abrir modal para editar
    $(document).on('click', '.edit-btn', async function () {
        const salaId = $(this).data('sala-id');
        isEditMode = true;
        currentSalaId = salaId;

        try {
            const response = await fetch(`/Sala/GetSala?id=${salaId}`);
            const data = await response.json();

            if (data.success) {
                $('#modalTitle').text('Editar Sala');
                $('#modalSubtitle').text('Modifica la información de la sala');
                $('#salaId').val(data.sala.id);
                $('#Numero').val(data.sala.numero);
                $('#Capacidad').val(data.sala.capacidad);
                $('#Ubicacion').val(data.sala.ubicacion);
                $('#Estado').val(data.sala.estado);
                $('#salaModal').fadeIn(300);
            } else {
                showMessage('Error al cargar la sala', 'error');
            }
        } catch (error) {
            showMessage('Error al cargar la sala', 'error');
        }
    });

    // Cerrar modal
    $('#closeModal, #btnCancel').on('click', function () {
        $('#salaModal').fadeOut(300);
        $('#salaForm')[0].reset();
        $('#modalMessage').hide();
    });

    // Cerrar modal al hacer clic fuera
    $('#salaModal').on('click', function (e) {
        if ($(e.target).is('#salaModal')) {
            $(this).fadeOut(300);
            $('#salaForm')[0].reset();
            $('#modalMessage').hide();
        }
    });

    // Enviar formulario
    $('#salaForm').on('submit', async function (e) {
        e.preventDefault();

        const formData = {
            Numero: $('#Numero').val(),
            Capacidad: parseInt($('#Capacidad').val()),
            Ubicacion: $('#Ubicacion').val(),
            Estado: parseInt($('#Estado').val()),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        const url = isEditMode ? `/Sala/Edit?id=${currentSalaId}` : '/Sala/Create';
        const method = 'POST';

        $('#btnSave').prop('disabled', true).text('Guardando...');

        try {
            const response = await fetch(url, {
                method: method,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': formData.__RequestVerificationToken
                },
                body: new URLSearchParams(formData).toString()
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                $('#salaModal').fadeOut(300);
                setTimeout(() => {
                    location.reload();
                }, 1000);
            } else {
                showMessage(data.message, 'error', '#modalMessage');
            }
        } catch (error) {
            showMessage('Ocurrió un error al guardar la sala', 'error', '#modalMessage');
        } finally {
            $('#btnSave').prop('disabled', false).text('Guardar');
        }
    });

    // Eliminar sala
    $(document).on('click', '.delete-btn', async function () {
        const salaId = $(this).data('sala-id');
        const salaNombre = $(this).closest('tr').find('.sala-nombre').text();

        if (!confirm(`¿Estás seguro de que deseas eliminar la sala "${salaNombre}"?`)) {
            return;
        }

        try {
            const response = await fetch('/Sala/Delete', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: `id=${salaId}&__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                $(`tr[data-sala-id="${salaId}"]`).fadeOut(300, function () {
                    $(this).remove();
                    if ($('#salasTableBody tr').length === 0) {
                        $('#salasTableBody').html('<tr><td colspan="6" class="no-data">No hay salas registradas</td></tr>');
                    }
                });
            } else {
                showMessage(data.message, 'error');
            }
        } catch (error) {
            showMessage('Ocurrió un error al eliminar la sala', 'error');
        }
    });

    // Búsqueda
    $('#searchInput').on('keyup', function () {
        const searchTerm = $(this).val().toLowerCase();
        filterTable(searchTerm, $('#filterEstado').val());
    });

    // Filtro por estado
    $('#filterEstado').on('change', function () {
        filterTable($('#searchInput').val().toLowerCase(), $(this).val());
    });

    function filterTable(searchTerm, estadoFilter) {
        $('#salasTableBody tr').each(function () {
            const nombre = $(this).find('.sala-nombre').text().toLowerCase();
            const ubicacion = $(this).find('td').eq(2).text().toLowerCase();
            const estado = $(this).find('.status-badge').hasClass('status-active') ? '1' : '0';

            const matchesSearch = nombre.includes(searchTerm) || ubicacion.includes(searchTerm);
            const matchesEstado = !estadoFilter || estado === estadoFilter;

            $(this).toggle(matchesSearch && matchesEstado);
        });
    }

    function showMessage(message, type, container = '#messageContainer') {
        const $container = $(container);
        $container.removeClass('message-success message-error')
            .addClass(type === 'success' ? 'message-success' : 'message-error')
            .text(message)
            .fadeIn(300);

        if (type === 'success') {
            setTimeout(() => {
                $container.fadeOut(300);
            }, 3000);
        }
    }
});

