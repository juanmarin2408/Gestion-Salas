$(document).ready(function () {
    let currentEquipoId = null;

    // Abrir modal de asignar
    $(document).on('click', '.btn-asignar', function () {
        currentEquipoId = $(this).data('equipo-id');
        const equipoCodigo = $(this).closest('tr').find('.equipo-codigo').text();
        $('#asignarSubtitle').text(`Selecciona el usuario para asignar el equipo ${equipoCodigo}`);
        $('#asignarEquipoId').val(currentEquipoId);
        $('#asignarForm')[0].reset();
        $('#UsuarioId').val('');
        $('#asignarModal').fadeIn(300);
    });

    // Cerrar modal de asignar
    $('#closeAsignarModal, #btnCancelAsignar').on('click', function () {
        $('#asignarModal').fadeOut(300);
        $('#asignarForm')[0].reset();
        $('#modalMessageAsignar').hide();
    });

    // Cerrar modal al hacer clic fuera
    $('#asignarModal').on('click', function (e) {
        if ($(e.target).is('#asignarModal')) {
            $(this).fadeOut(300);
            $('#asignarForm')[0].reset();
            $('#modalMessageAsignar').hide();
        }
    });

    // Enviar formulario de asignar
    $('#asignarForm').on('submit', async function (e) {
        e.preventDefault();

        const formData = {
            id: $('#asignarEquipoId').val(),
            UsuarioId: $('#UsuarioId').val(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        $('#btnAsignar').prop('disabled', true).text('Asignando...');

        try {
            const response = await fetch('/Coordinador/AsignarEquipo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': formData.__RequestVerificationToken
                },
                body: new URLSearchParams(formData).toString()
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                $('#asignarModal').fadeOut(300);
                setTimeout(() => {
                    location.reload();
                }, 1000);
            } else {
                showMessage(data.message, 'error', '#modalMessageAsignar');
            }
        } catch (error) {
            showMessage('Ocurrió un error al asignar el equipo', 'error', '#modalMessageAsignar');
        } finally {
            $('#btnAsignar').prop('disabled', false).text('Asignar Equipo');
        }
    });

    // Abrir modal de bloquear (solo botones de la tabla, no el del formulario)
    $(document).on('click', 'table .btn-bloquear', function (e) {
        e.preventDefault();
        e.stopPropagation();
        currentEquipoId = $(this).data('equipo-id');
        const equipoCodigo = $(this).closest('tr').find('.equipo-codigo').text();
        $('#bloquearSubtitle').text(`Ingresa el motivo y prioridad para bloquear el equipo ${equipoCodigo}`);
        $('#bloquearEquipoId').val(currentEquipoId);
        $('#bloquearForm')[0].reset();
        $('#MotivoBloqueo').val('');
        $('#PrioridadBloqueo').val('1');
        $('#modalMessageBloquear').hide();
        $('#bloquearModal').fadeIn(300);
    });

    // Cerrar modal de bloquear
    $('#closeBloquearModal, #btnCancelBloquear').on('click', function () {
        $('#bloquearModal').fadeOut(300);
        $('#bloquearForm')[0].reset();
        $('#modalMessageBloquear').hide();
    });

    // Cerrar modal al hacer clic fuera
    $('#bloquearModal').on('click', function (e) {
        if ($(e.target).is('#bloquearModal')) {
            $(this).fadeOut(300);
            $('#bloquearForm')[0].reset();
            $('#modalMessageBloquear').hide();
        }
    });

    // Enviar formulario de bloquear
    $('#bloquearForm').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        console.log('Form submit triggered');

        const motivoBloqueo = $('#MotivoBloqueo').val();
        const equipoId = $('#bloquearEquipoId').val();
        
        console.log('Motivo:', motivoBloqueo);
        console.log('Equipo ID:', equipoId);

        if (!motivoBloqueo || motivoBloqueo.trim() === '') {
            showMessage('El motivo del bloqueo es requerido', 'error', '#modalMessageBloquear');
            return false;
        }

        if (!equipoId || equipoId === '') {
            showMessage('El ID del equipo es requerido', 'error', '#modalMessageBloquear');
            return false;
        }

        const formData = {
            id: equipoId,
            MotivoBloqueo: motivoBloqueo.trim(),
            PrioridadBloqueo: parseInt($('#PrioridadBloqueo').val()),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        const $btnBloquear = $('#btnBloquearEquipo');
        const originalText = $btnBloquear.text();
        $btnBloquear.prop('disabled', true).text('Bloqueando...');
        $('#modalMessageBloquear').hide();

        // Prevenir que el formulario se resetee
        $(this).off('reset');

        try {
            const params = new URLSearchParams();
            params.append('id', formData.id);
            params.append('MotivoBloqueo', formData.MotivoBloqueo);
            params.append('PrioridadBloqueo', formData.PrioridadBloqueo.toString());
            params.append('__RequestVerificationToken', formData.__RequestVerificationToken);

            console.log('Sending request with:', {
                id: formData.id,
                MotivoBloqueo: formData.MotivoBloqueo,
                PrioridadBloqueo: formData.PrioridadBloqueo
            });

            const response = await fetch('/Coordinador/BloquearEquipo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': formData.__RequestVerificationToken
                },
                body: params.toString()
            });

            console.log('Response status:', response.status);

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error response:', errorText);
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            console.log('Response data:', data);

            if (data.success) {
                showMessage(data.message, 'success');
                setTimeout(() => {
                    $('#bloquearModal').fadeOut(300, function() {
                        location.reload();
                    });
                }, 500);
            } else {
                showMessage(data.message || 'Error al bloquear el equipo', 'error', '#modalMessageBloquear');
                $btnBloquear.prop('disabled', false).text(originalText);
            }
        } catch (error) {
            console.error('Error al bloquear equipo:', error);
            showMessage('Ocurrió un error al bloquear el equipo. Por favor intenta de nuevo. Error: ' + error.message, 'error', '#modalMessageBloquear');
            $btnBloquear.prop('disabled', false).text(originalText);
        }

        return false;
    });

    // Liberar equipo
    $(document).on('click', '.btn-liberar', async function () {
        const equipoId = $(this).data('equipo-id');
        const equipoCodigo = $(this).closest('tr').find('.equipo-codigo').text();
        
        if (!confirm(`¿Estás seguro de que deseas liberar el equipo ${equipoCodigo}?`)) {
            return;
        }

        try {
            const response = await fetch('/Coordinador/LiberarEquipo', {
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
                setTimeout(() => {
                    location.reload();
                }, 1000);
            } else {
                showMessage(data.message, 'error');
            }
        } catch (error) {
            showMessage('Ocurrió un error al liberar el equipo', 'error');
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
        $('#equiposTableBody tr').each(function () {
            const codigo = $(this).find('.equipo-codigo').text().toLowerCase();
            const sala = $(this).find('td').eq(1).text().toLowerCase();
            const estado = $(this).data('estado')?.toString() || '';

            const matchesSearch = codigo.includes(searchTerm) || sala.includes(searchTerm);
            let matchesEstado = true;
            
            if (estadoFilter === '2') {
                // Bloqueados incluye EnMantenimiento (2) y Dañado (3)
                matchesEstado = estado === '2' || estado === '3';
            } else if (estadoFilter) {
                matchesEstado = estado === estadoFilter;
            }

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

