$(document).ready(function () {
    console.log('✓ Script coordinador-reportes.js cargado correctamente');
    let currentReporteId = null;

    // Abrir modal de atender (solo botones de la tabla, no el botón de submit del modal)
    $(document).on('click', 'table .btn-atender', function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        console.log('🔵 Click en botón "Atender" detectado');
        
        // Leer los atributos data-* directamente del elemento HTML usando getAttribute
        const btnElement = this; // Elemento DOM nativo
        const $btn = $(this);
        
        // Leer directamente del atributo HTML (más confiable)
        currentReporteId = btnElement.getAttribute('data-reporte-id') || 
                          $btn.attr('data-reporte-id') || 
                          $btn.data('reporte-id') || 
                          $btn.data('reporteId');
        
        const codigo = btnElement.getAttribute('data-codigo') || 
                      $btn.attr('data-codigo') || 
                      $btn.data('codigo') || 
                      'Sin código';
        
        const usuario = btnElement.getAttribute('data-usuario') || 
                       $btn.attr('data-usuario') || 
                       $btn.data('usuario') || 
                       'Usuario desconocido';
        
        const descripcion = btnElement.getAttribute('data-descripcion') || 
                           $btn.attr('data-descripcion') || 
                           $btn.data('descripcion') || 
                           'Sin descripción';
        
        console.log('Datos del botón (getAttribute - método nativo):', {
            'data-reporte-id': btnElement.getAttribute('data-reporte-id'),
            'data-codigo': btnElement.getAttribute('data-codigo'),
            'data-usuario': btnElement.getAttribute('data-usuario'),
            'data-descripcion': btnElement.getAttribute('data-descripcion')
        });
        
        console.log('Datos procesados:', {
            reporteId: currentReporteId,
            codigo: codigo,
            usuario: usuario,
            descripcion: descripcion
        });
        
        if (!currentReporteId || currentReporteId === 'null' || currentReporteId === 'undefined') {
            console.error('❌ ERROR: No se pudo obtener el ID del reporte');
            console.error('Elemento HTML completo:', btnElement.outerHTML);
            console.error('Todos los atributos data-*:', {
                'data-reporte-id': btnElement.getAttribute('data-reporte-id'),
                'data-codigo': btnElement.getAttribute('data-codigo'),
                'data-usuario': btnElement.getAttribute('data-usuario'),
                'data-descripcion': btnElement.getAttribute('data-descripcion')
            });
            alert('Error: No se pudo obtener el ID del reporte. Por favor recarga la página y revisa la consola para más detalles.');
            return false;
        }
        
        // Guardar el ID en el campo oculto
        $('#atenderReporteId').val(currentReporteId);
        
        // Mostrar los datos en el modal (manejar valores null/undefined)
        $('#modalCodigo').text(codigo && codigo !== 'null' && codigo !== 'undefined' ? codigo : 'Sin código');
        $('#modalUsuario').text(usuario && usuario !== 'null' && usuario !== 'undefined' ? usuario : 'Usuario desconocido');
        $('#modalDescripcion').text(descripcion && descripcion !== 'null' && descripcion !== 'undefined' ? descripcion : 'Sin descripción');
        $('#atenderSubtitle').text(`Ingresa la solución aplicada para el reporte`);
        
        // Limpiar solo el textarea, no resetear todo el formulario para preservar el token
        $('#SolucionAplicada').val('');
        $('#modalMessageAtender').hide();
        $('#btnAtenderReporte').prop('disabled', false);
        $('#atenderModal').fadeIn(300);
        
        const reporteIdEnCampo = $('#atenderReporteId').val();
        console.log('✓ Modal abierto. ID del reporte en el campo oculto:', reporteIdEnCampo);
        
        if (!reporteIdEnCampo || reporteIdEnCampo === 'null' || reporteIdEnCampo === 'undefined') {
            console.error('❌ ERROR CRÍTICO: El ID del reporte no se guardó en el campo oculto');
            console.error('Valor intentado:', currentReporteId);
            console.error('Valor en campo:', reporteIdEnCampo);
            alert('Error crítico: No se pudo guardar el ID del reporte. Por favor recarga la página.');
        } else {
            console.log('✅ ID del reporte guardado correctamente:', reporteIdEnCampo);
        }
    });

    // Cerrar modal de atender
    $('#closeAtenderModal, #btnCancelAtender').on('click', function () {
        $('#atenderModal').fadeOut(300);
        $('#SolucionAplicada').val('');
        $('#modalMessageAtender').hide();
        $('#btnAtenderReporte').prop('disabled', false);
    });

    // Cerrar modal al hacer clic fuera
    $('#atenderModal').on('click', function (e) {
        if ($(e.target).is('#atenderModal')) {
            $(this).fadeOut(300);
            $('#SolucionAplicada').val('');
            $('#modalMessageAtender').hide();
            $('#btnAtenderReporte').prop('disabled', false);
        }
    });

    // Enviar formulario de atender
    $('#atenderForm').off('submit').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        console.log('=== INICIO: Enviar formulario de atender reporte ===');
        
        const solucionAplicada = $('#SolucionAplicada').val().trim();
        
        // Leer el ID del campo oculto (método principal)
        let reporteId = $('#atenderReporteId').val();
        console.log('ID leído del campo oculto (#atenderReporteId):', reporteId);
        
        // Si no está en el campo oculto, intentar usar la variable global
        if (!reporteId || reporteId === '' || reporteId === 'null' || reporteId === 'undefined') {
            if (currentReporteId) {
                reporteId = currentReporteId;
                $('#atenderReporteId').val(reporteId);
                console.log('⚠️ ID del reporte recuperado de la variable global:', reporteId);
            } else {
                console.error('❌ No se encontró el ID del reporte ni en el campo oculto ni en la variable global');
            }
        }

        console.log('Datos del formulario:', {
            reporteId: reporteId,
            reporteIdTipo: typeof reporteId,
            reporteIdLongitud: reporteId ? reporteId.length : 0,
            solucionAplicada: solucionAplicada ? solucionAplicada.substring(0, 50) + '...' : '(vacío)',
            longitudSolucion: solucionAplicada ? solucionAplicada.length : 0,
            currentReporteId: currentReporteId
        });

        // Validación client-side
        if (!solucionAplicada) {
            const errorMsg = 'Por favor ingresa la solución aplicada';
            console.error('VALIDACIÓN FALLIDA:', errorMsg);
            showMessage(errorMsg, 'error', '#modalMessageAtender');
            $('#SolucionAplicada').focus();
            return false;
        }

        if (!reporteId || reporteId === '' || reporteId === 'undefined') {
            const errorMsg = 'Error: No se pudo identificar el reporte. ID: ' + (reporteId || 'null');
            console.error('VALIDACIÓN FALLIDA:', errorMsg);
            console.error('Estado actual:', {
                reporteIdEnCampo: $('#atenderReporteId').val(),
                currentReporteId: currentReporteId,
                todosLosCamposOcultos: $('input[type="hidden"]').map(function() { return $(this).attr('id') + ': ' + $(this).val(); }).get()
            });
            showMessage('Error crítico: No se pudo obtener el ID del reporte. Por favor cierra el modal, recarga la página e intenta de nuevo.', 'error', '#modalMessageAtender');
            return false;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();
        console.log('Token encontrado:', token ? 'SÍ (longitud: ' + token.length + ')' : 'NO');
        
        if (!token) {
            const errorMsg = 'Error de seguridad: Token no encontrado. Por favor recarga la página.';
            console.error('ERROR DE SEGURIDAD:', errorMsg);
            showMessage(errorMsg, 'error', '#modalMessageAtender');
            return false;
        }

        const $btnAtender = $('#btnAtenderReporte');
        const originalText = $btnAtender.text();
        $btnAtender.prop('disabled', true).text('Atendiendo...');
        $('#modalMessageAtender').hide();

        try {
            const formData = new URLSearchParams();
            formData.append('id', reporteId);
            formData.append('Observaciones', solucionAplicada);
            formData.append('__RequestVerificationToken', token);

            console.log('Enviando petición POST a /Coordinador/AtenderReporte');
            console.log('Body:', formData.toString().substring(0, 200) + '...');

            const response = await fetch('/Coordinador/AtenderReporte', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: formData.toString()
            });

            console.log('Respuesta recibida:', {
                status: response.status,
                statusText: response.statusText,
                ok: response.ok,
                headers: Object.fromEntries(response.headers.entries())
            });

            let responseText = '';
            let data;

            try {
                responseText = await response.text();
                console.log('Contenido de la respuesta (texto):', responseText);
                
                if (!responseText || responseText.trim() === '') {
                    throw new Error('La respuesta del servidor está vacía');
                }

                data = JSON.parse(responseText);
                console.log('Datos parseados (JSON):', data);
            } catch (jsonError) {
                console.error('ERROR AL PARSEAR RESPUESTA:', jsonError);
                console.error('Respuesta recibida (texto completo):', responseText);
                
                let errorMessage = 'Error al procesar la respuesta del servidor.';
                
                if (response.status === 400) {
                    errorMessage = 'Error de validación (400). ' + (responseText || 'Por favor recarga la página e intenta de nuevo.');
                } else if (response.status === 401) {
                    errorMessage = 'Error de autenticación (401). Por favor inicia sesión nuevamente.';
                } else if (response.status === 403) {
                    errorMessage = 'Error de autorización (403). No tienes permisos para esta acción.';
                } else if (response.status === 500) {
                    errorMessage = 'Error del servidor (500). ' + (responseText || 'Por favor intenta más tarde.');
                } else {
                    errorMessage = 'Error HTTP ' + response.status + ': ' + (responseText || 'Error desconocido');
                }
                
                showMessage(errorMessage, 'error', '#modalMessageAtender');
                $btnAtender.prop('disabled', false).text(originalText);
                return false;
            }

            console.log('Procesando respuesta:', data);

            if (data.success) {
                console.log('✓ ÉXITO: Reporte atendido correctamente');
                showMessage(data.message || 'Reporte atendido exitosamente', 'success');
                $('#atenderModal').fadeOut(300);
                
                // Recargar la página después de un breve delay
                console.log('Recargando página en 500ms...');
                setTimeout(() => {
                    location.reload();
                }, 500);
            } else {
                const errorMsg = data.message || 'Error al atender el reporte';
                console.error('✗ ERROR EN RESPUESTA:', errorMsg);
                showMessage(errorMsg, 'error', '#modalMessageAtender');
                $btnAtender.prop('disabled', false).text(originalText);
            }
        } catch (error) {
            console.error('✗ EXCEPCIÓN CAPTURADA:', error);
            console.error('Stack trace:', error.stack);
            const errorMsg = 'Error: ' + (error.message || 'Ocurrió un error inesperado. Revisa la consola para más detalles.');
            showMessage(errorMsg, 'error', '#modalMessageAtender');
            $btnAtender.prop('disabled', false).text(originalText);
        }

        console.log('=== FIN: Enviar formulario de atender reporte ===');
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
        
        // Log en consola también
        if (type === 'error') {
            console.error('MENSAJE DE ERROR:', message);
        } else {
            console.log('MENSAJE DE ÉXITO:', message);
        }
        
        $container.removeClass('message-success message-error')
            .addClass(type === 'success' ? 'message-success' : 'message-error')
            .html('<strong>' + (type === 'error' ? '⚠️ ' : '✓ ') + message + '</strong>')
            .css({
                'display': 'block',
                'padding': '12px',
                'margin': '10px 0',
                'border-radius': '4px',
                'font-weight': 'bold'
            })
            .fadeIn(300);

        if (type === 'success') {
            setTimeout(() => {
                $container.fadeOut(300);
            }, 3000);
        } else {
            // Los errores no se ocultan automáticamente
            // El usuario debe cerrarlos manualmente o se ocultan al cerrar el modal
        }
    }
});

