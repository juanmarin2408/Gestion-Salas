$(document).ready(function () {
    let currentReporteId = null;

    // Abrir modal de atender
    $(document).on('click', '.btn-atender', function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        currentReporteId = $(this).data('reporte-id');
        const codigo = $(this).data('codigo');
        const usuario = $(this).data('usuario');
        const descripcion = $(this).data('descripcion');
        
        $('#atenderReporteId').val(currentReporteId);
        $('#modalCodigo').text(codigo);
        $('#modalUsuario').text(usuario);
        $('#modalDescripcion').text(descripcion);
        $('#atenderSubtitle').text(`Ingresa la solución aplicada para el reporte`);
        $('#atenderForm')[0].reset();
        $('#SolucionAplicada').val('');
        $('#modalMessageAtender').hide();
        $('#atenderModal').fadeIn(300);
    });

    // Cerrar modal de atender
    $('#closeAtenderModal, #btnCancelAtender').on('click', function () {
        $('#atenderModal').fadeOut(300);
        $('#atenderForm')[0].reset();
        $('#modalMessageAtender').hide();
    });

    // Cerrar modal al hacer clic fuera
    $('#atenderModal').on('click', function (e) {
        if ($(e.target).is('#atenderModal')) {
            $(this).fadeOut(300);
            $('#atenderForm')[0].reset();
            $('#modalMessageAtender').hide();
        }
    });

    // Enviar formulario de atender
    $('#atenderForm').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const solucionAplicada = $('#SolucionAplicada').val();
        if (!solucionAplicada || solucionAplicada.trim() === '') {
            showMessage('La solución aplicada es requerida', 'error', '#modalMessageAtender');
            return false;
        }

        const reporteId = $('#atenderReporteId').val();
        if (!reporteId || reporteId === '') {
            showMessage('El ID del reporte es requerido', 'error', '#modalMessageAtender');
            return false;
        }

        const formData = {
            id: reporteId,
            Observaciones: solucionAplicada.trim(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        const $btnAtender = $('#btnAtenderReporte');
        const originalText = $btnAtender.text();
        $btnAtender.prop('disabled', true).text('Atendiendo...');
        $('#modalMessageAtender').hide();

        try {
            const params = new URLSearchParams();
            params.append('id', formData.id);
            params.append('Observaciones', formData.Observaciones);
            params.append('__RequestVerificationToken', formData.__RequestVerificationToken);

            const response = await fetch('/Coordinador/AtenderReporte', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': formData.__RequestVerificationToken
                },
                body: params.toString()
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error response:', errorText);
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                setTimeout(() => {
                    $('#atenderModal').fadeOut(300, function() {
                        location.reload();
                    });
                }, 500);
            } else {
                showMessage(data.message || 'Error al atender el reporte', 'error', '#modalMessageAtender');
                $btnAtender.prop('disabled', false).text(originalText);
            }
        } catch (error) {
            console.error('Error al atender reporte:', error);
            showMessage('Ocurrió un error al atender el reporte. Por favor intenta de nuevo.', 'error', '#modalMessageAtender');
            $btnAtender.prop('disabled', false).text(originalText);
        }

        return false;
    });

    // Búsqueda
    $('#searchInput').on('keyup', function () {
        const searchTerm = $(this).val().toLowerCase();
        filterTable(searchTerm, $('#filterEstado').val(), $('#filterPrioridad').val());
    });

    // Filtro por estado
    $('#filterEstado').on('change', function () {
        filterTable($('#searchInput').val().toLowerCase(), $(this).val(), $('#filterPrioridad').val());
    });

    // Filtro por prioridad
    $('#filterPrioridad').on('change', function () {
        filterTable($('#searchInput').val().toLowerCase(), $('#filterEstado').val(), $(this).val());
    });

    function filterTable(searchTerm, estadoFilter, prioridadFilter) {
        $('#reportesTableBody tr').each(function () {
            const codigo = $(this).find('td:eq(1)').text().toLowerCase();
            const usuario = $(this).find('td:eq(2)').text().toLowerCase();
            const descripcion = $(this).find('td:eq(3)').text().toLowerCase();
            const estado = $(this).data('estado')?.toString() || '';
            const prioridad = $(this).data('prioridad')?.toString() || '';

            const matchesSearch = codigo.includes(searchTerm) || usuario.includes(searchTerm) || descripcion.includes(searchTerm);
            const matchesEstado = !estadoFilter || estado === estadoFilter;
            const matchesPrioridad = !prioridadFilter || prioridad === prioridadFilter;

            $(this).toggle(matchesSearch && matchesEstado && matchesPrioridad);
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

