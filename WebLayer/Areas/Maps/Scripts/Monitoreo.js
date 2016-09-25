$(function () {
    // Declare a proxy to reference the hub.
    var chat = $.connection.switchAndCams;
    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (port, status) {
        //emitter.emit('something');
        if (status == 2) {
            if (port == 1)
                swal("Nuevo estado", "Se pierde la conexión en el hilo 6 desde la carrera 18 Este con calle 40 hasta la Carrera 16 Este con Calle 37", "error");
            if (port == 2)
                swal("Nuevo estado", "Se pierde la conexión en el hilo 1 Sobre la carrera 18 Este, desde la Calle 40 a la Calle 43", "error");
        }
        else {
            if (port == 1)
                swal("Nuevo estado", "Se restablece la conexión en el hilo 6 desde la carrera 18 Este con calle 40 hasta la Carrera 16 Este con Calle 37", "success");
            if (port == 2)
                swal("Nuevo estado", "Se restablece la conexión en el hilo 1 Sobre la carrera 18 Este, desde la Calle 40 a la Calle 43", "success");
        }
            
    };

    chat.client.broadcastMessageCam = function (cam, status) {
        //emitter.emit('something');
        if (status == 2) {
            //if (port == 4)
            //    selectcam("Cam4");
            swal("Nuevo estado", "La Camara " + cam + " se ha desconectado", "error");
        }
        else {
            //if (port == 4)
            //    selectcam("Camaa");
            swal("Nuevo estado", "La Camara " + cam + " se ha conectado", "success");
        }

    };

    // Get the user name and store it to prepend to messages.
    
    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            chat.server.send($('#displayname').val(), $('#message').val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
    });
});
function myFunction() {
    swal("Nuevo estado", "El puerto " , "success");
}