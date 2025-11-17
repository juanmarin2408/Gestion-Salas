$(document).ready(function () {
    let isEditMode = false;
    let currentEquipoId = null;

    // Abrir modal para crear
    $('#btnNuevoEquipo').on('click', function () {
        isEditMode = false;
        currentEquipoId = null;
        $('#modalTitle').text('Registrar Nuevo Equipo');
        $('#modalSubtitle').text('Ingresa la información del equipo de cómputo');
        $('#equipoForm')[0].reset();
        $('#equipoId').val('');
        $('#Estado').val('0');
        $('#equipoModal').fadeIn(300);
    });

    // Abrir modal para editar
    $(document).on('click', '.edit-btn', async function () {
        const equipoId = $(this).data('equipo-id');
        isEditMode = true;
        currentEquipoId = equipoId;

        try {
            const response = await fetch(`/Equipo/GetEquipo?id=${equipoId}`);
            const data = await response.json();

            if (data.success) {
                $('#modalTitle').text('Editar Equipo');
                $('#modalSubtitle').text('Modifica la información del equipo');
                $('#equipoId').val(data.equipo.id);
                $('#SalaId').val(data.equipo.salaId);
                $('#Estado').val(data.equipo.estado);
                $('#equipoModal').fadeIn(300);
            } else {
                showMessage('Error al cargar el equipo', 'error');
            }
        } catch (error) {
            showMessage('Error al cargar el equipo', 'error');
        }
    });

    // Cerrar modal
    $('#closeModal, #btnCancel').on('click', function () {
        $('#equipoModal').fadeOut(300);
        $('#equipoForm')[0].reset();
        $('#modalMessage').hide();
    });

    // Cerrar modal al hacer clic fuera
    $('#equipoModal').on('click', function (e) {
        if ($(e.target).is('#equipoModal')) {
            $(this).fadeOut(300);
            $('#equipoForm')[0].reset();
            $('#modalMessage').hide();
        }
    });

    // Enviar formulario
    $('#equipoForm').on('submit', async function (e) {
        e.preventDefault();

        const formData = {
            SalaId: $('#SalaId').val(),
            Estado: parseInt($('#Estado').val()),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        const url = isEditMode ? `/Equipo/Edit?id=${currentEquipoId}` : '/Equipo/Create';
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
                $('#equipoModal').fadeOut(300);
                setTimeout(() => {
                    location.reload();
                }, 1000);
            } else {
                showMessage(data.message, 'error', '#modalMessage');
            }
        } catch (error) {
            showMessage('Ocurrió un error al guardar el equipo', 'error', '#modalMessage');
        } finally {
            $('#btnSave').prop('disabled', false).text('Guardar');
        }
    });

    // Eliminar equipo
    $(document).on('click', '.delete-btn', async function () {
        const equipoId = $(this).data('equipo-id');
        const equipoCodigo = $(this).closest('tr').find('.equipo-codigo').text();

        if (!confirm(`¿Estás seguro de que deseas eliminar el equipo "${equipoCodigo}"?`)) {
            return;
        }

        try {
            const response = await fetch('/Equipo/Delete', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: `id=${equipoId}&__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                $(`tr[data-equipo-id="${equipoId}"]`).fadeOut(300, function () {
                    $(this).remove();
                    if ($('#equiposTableBody tr').length === 0) {
                        $('#equiposTableBody').html('<tr><td colspan="4" class="no-data">No hay equipos registrados</td></tr>');
                    }
                });
            } else {
                showMessage(data.message, 'error');
            }
        } catch (error) {
            showMessage('Ocurrió un error al eliminar el equipo', 'error');
        }
    });

    // Búsqueda
    $('#searchInput').on('keyup', function () {
        const searchTerm = $(this).val().toLowerCase();
        filterTable(searchTerm, $('#filterSala').val(), $('#filterEstado').val());
    });

    // Filtro por sala
    $('#filterSala').on('change', function () {
        filterTable($('#searchInput').val().toLowerCase(), $(this).val(), $('#filterEstado').val());
    });

    // Filtro por estado
    $('#filterEstado').on('change', function () {
        filterTable($('#searchInput').val().toLowerCase(), $('#filterSala').val(), $(this).val());
    });

    function filterTable(searchTerm, salaFilter, estadoFilter) {
        $('#equiposTableBody tr').each(function () {
            const codigo = $(this).find('.equipo-codigo').text().toLowerCase();
            const sala = $(this).find('td').eq(1).text().toLowerCase();
            const estadoText = $(this).find('.status-badge').text().toLowerCase();
            const estadoValue = $(this).find('.status-badge').data('estado') || '';

            const matchesSearch = codigo.includes(searchTerm) || sala.includes(searchTerm);
            const matchesSala = !salaFilter || $(this).data('sala-id') === salaFilter;
            const matchesEstado = !estadoFilter || estadoValue === estadoFilter;

            $(this).toggle(matchesSearch && matchesSala && matchesEstado);
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

