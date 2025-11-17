$(document).ready(function () {
    let isEditMode = false;
    let currentUsuarioId = null;

    // Abrir modal para crear
    $('#btnNuevoUsuario').on('click', function () {
        isEditMode = false;
        currentUsuarioId = null;
        $('#modalTitle').text('Crear Nuevo Usuario');
        $('#modalSubtitle').text('Ingresa la información del usuario');
        $('#usuarioForm')[0].reset();
        $('#usuarioId').val('');
        $('#Rol').val('1');
        $('#Password').prop('required', true);
        $('#ConfirmPassword').prop('required', true);
        $('#confirmPasswordField').show();
        $('#usuarioModal').fadeIn(300);
    });

    // Abrir modal para editar
    $(document).on('click', '.edit-btn', async function () {
        const usuarioId = $(this).data('usuario-id');
        isEditMode = true;
        currentUsuarioId = usuarioId;

        try {
            const response = await fetch(`/Usuario/GetUsuario?id=${usuarioId}`);
            const data = await response.json();

            if (data.success) {
                $('#modalTitle').text('Editar Usuario');
                $('#modalSubtitle').text('Modifica la información del usuario');
                $('#usuarioId').val(data.usuario.id);
                $('#Nombre').val(data.usuario.nombre);
                $('#Apellido').val(data.usuario.apellido || '');
                $('#Documento').val(data.usuario.documento);
                $('#Email').val(data.usuario.email);
                $('#Rol').val(data.usuario.rol);
                $('#Password').prop('required', false);
                $('#ConfirmPassword').prop('required', false);
                $('#confirmPasswordField').hide();
                $('#usuarioModal').fadeIn(300);
            } else {
                showMessage('Error al cargar el usuario', 'error');
            }
        } catch (error) {
            showMessage('Error al cargar el usuario', 'error');
        }
    });

    // Cerrar modal
    $('#closeModal, #btnCancel').on('click', function () {
        $('#usuarioModal').fadeOut(300);
        $('#usuarioForm')[0].reset();
        $('#modalMessage').hide();
        $('#confirmPasswordField').show();
    });

    // Cerrar modal al hacer clic fuera
    $('#usuarioModal').on('click', function (e) {
        if ($(e.target).is('#usuarioModal')) {
            $(this).fadeOut(300);
            $('#usuarioForm')[0].reset();
            $('#modalMessage').hide();
            $('#confirmPasswordField').show();
        }
    });

    // Enviar formulario
    $('#usuarioForm').on('submit', async function (e) {
        e.preventDefault();

        // Validar contraseñas si están presentes
        if ($('#Password').val() && $('#Password').val() !== $('#ConfirmPassword').val()) {
            showMessage('Las contraseñas no coinciden', 'error', '#modalMessage');
            return;
        }

        const formData = {
            Nombre: $('#Nombre').val(),
            Apellido: $('#Apellido').val() || '',
            Documento: $('#Documento').val(),
            Email: $('#Email').val(),
            Password: $('#Password').val() || '',
            ConfirmPassword: $('#ConfirmPassword').val() || '',
            Rol: parseInt($('#Rol').val()),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        const url = isEditMode ? `/Usuario/Edit?id=${currentUsuarioId}` : '/Usuario/Create';
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
                $('#usuarioModal').fadeOut(300);
                setTimeout(() => {
                    location.reload();
                }, 1000);
            } else {
                showMessage(data.message, 'error', '#modalMessage');
            }
        } catch (error) {
            showMessage('Ocurrió un error al guardar el usuario', 'error', '#modalMessage');
        } finally {
            $('#btnSave').prop('disabled', false).text('Guardar');
        }
    });

    // Eliminar usuario
    $(document).on('click', '.delete-btn', async function () {
        const usuarioId = $(this).data('usuario-id');
        const usuarioNombre = $(this).closest('tr').find('.usuario-nombre').text();

        if (!confirm(`¿Estás seguro de que deseas eliminar el usuario "${usuarioNombre}"?`)) {
            return;
        }

        try {
            const response = await fetch('/Usuario/Delete', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: `id=${usuarioId}&__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                $(`tr[data-usuario-id="${usuarioId}"]`).fadeOut(300, function () {
                    $(this).remove();
                    if ($('#usuariosTableBody tr').length === 0) {
                        $('#usuariosTableBody').html('<tr><td colspan="6" class="no-data">No hay usuarios registrados</td></tr>');
                    }
                });
            } else {
                showMessage(data.message, 'error');
            }
        } catch (error) {
            showMessage('Ocurrió un error al eliminar el usuario', 'error');
        }
    });

    // Búsqueda
    $('#searchInput').on('keyup', function () {
        const searchTerm = $(this).val().toLowerCase();
        filterTable(searchTerm, $('#filterRol').val(), $('#filterEstado').val());
    });

    // Filtro por rol
    $('#filterRol').on('change', function () {
        filterTable($('#searchInput').val().toLowerCase(), $(this).val(), $('#filterEstado').val());
    });

    // Filtro por estado
    $('#filterEstado').on('change', function () {
        filterTable($('#searchInput').val().toLowerCase(), $('#filterRol').val(), $(this).val());
    });

    function filterTable(searchTerm, rolFilter, estadoFilter) {
        $('#usuariosTableBody tr').each(function () {
            const nombre = $(this).find('.usuario-nombre').text().toLowerCase();
            const correo = $(this).find('td').eq(1).text().toLowerCase();
            const rol = $(this).data('rol')?.toString() || '';
            const activo = $(this).data('activo')?.toString() || '';

            const matchesSearch = nombre.includes(searchTerm) || correo.includes(searchTerm);
            const matchesRol = !rolFilter || rol === rolFilter;
            const matchesEstado = !estadoFilter || activo === estadoFilter;

            $(this).toggle(matchesSearch && matchesRol && matchesEstado);
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

