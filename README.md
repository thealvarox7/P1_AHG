MEMORIA TÉCNICA: PROYECTO UNITY - PRÁCTICA 1
Autor: Álvaro Hernández García

I. DESARROLLO DEL CONTROLADOR DEL PERSONAJE

A) Vinculación del componente Rigidbody
Dentro del método Awake, se localiza y almacena la referencia del Rigidbody del jugador mediante la instrucción GetComponent. Esto permite gestionar posteriormente el desplazamiento y la ejecución de los saltos.

B) Captura de comandos de entrada
En el método Update, el sistema registra las pulsaciones de las teclas para los ejes horizontal y vertical. Estas lecturas permiten que el usuario controle el giro y el avance o retroceso. Además, se ha configurado la cámara para que siga de forma fluida el movimiento del personaje, permitiendo desplazamientos diagonales si se presionan varias teclas simultáneamente.

C) Determinación de la trayectoria
La dirección del movimiento se establece tomando como referencia el eje frontal del propio transform. De este modo, el personaje siempre se desplaza hacia adelante según su orientación actual en el espacio.

D) Gestión física del desplazamiento
Durante el ciclo de FixedUpdate, se define la velocidad actual y se traslada al jugador utilizando rb.MovePosition. El cálculo del desplazamiento final toma en cuenta tanto los inputs recibidos como la velocidad configurada.

E) Independencia de la tasa de cuadros (Framerate)
Para garantizar que el movimiento sea uniforme en cualquier ordenador, todas las operaciones físicas se ejecutan en FixedUpdate y se escalan utilizando Time.fixedDeltaTime.
II. MECÁNICAS DE SALTO MEDIANTE FÍSICAS

A) Configuración de la potencia
Se ha creado una variable expuesta en el inspector denominada fuerzaSalto, la cual permite ajustar la intensidad del impulso vertical de forma sencilla.

B) Validación de superficie mediante etiquetas
Para detectar si el jugador está tocando el suelo, se emplean los métodos de colisión estándar (OnCollisionEnter, Stay y Exit). El script verifica que el objeto con el que se contacta tenga asignado el Tag "Ground".

C) Control de estado del salto
Se utiliza una variable lógica (booleano) para determinar si el personaje está apoyado. El salto solo se activa si esta condición es verdadera y se presiona la tecla de espacio. En ese momento, se aplica un impulso vertical directo al Rigidbody.

D) Restricciones y salto doble
El sistema bloquea la posibilidad de saltar infinitamente mientras se está en el aire. No obstante, se ha incluido una función de doble salto que rastrea cuántas veces se ha saltado desde que se abandonó el suelo para permitir un segundo impulso extra.
III. INTEGRACIÓN DE LA CÁMARA

Se han seguido las pautas generales de la práctica, asegurando que el objeto de la cámara tenga asignado el tag Main Camera, requisito indispensable para el correcto funcionamiento del renderizado y el seguimiento.
IV. ESTRUCTURAS DE ESCENARIO BÁSICAS

Se han implementado las plataformas estándar siguiendo las instrucciones del ejercicio sin desviaciones técnicas reseñables.
V. PLATAFORMAS CON LÓGICA DE MOVIMIENTO

TIPO A: Plataformas con desplazamiento cíclico

Registro de coordenadas: Al iniciar el juego, se guarda el punto de origen de la plataforma.

Destino y visibilidad: Se define un objeto externo como meta, cuya representación visual se oculta durante la ejecución para que solo sirva como referencia invisible.

Fluidez y velocidad: El movimiento se calcula con Vector3.MoveTowards en el ciclo de FixedUpdate, garantizando un ritmo constante.

Lógica de retorno: Se emplea un interruptor lógico para alternar el destino. Cuando la plataforma alcanza el punto final, cambia su objetivo de vuelta al origen y viceversa.

TIPO B: Plataformas reactivas al contacto

Activación por peso: La plataforma detecta mediante OnCollisionEnter si el objeto que se ha posicionado sobre ella es el jugador.

Temporización: Una vez detectado el contacto, el sistema inicia un breve contador de espera antes de iniciar el movimiento.

Recuperación de posición: Tras realizar su función, la plataforma regresa a su ubicación inicial utilizando MoveTowards.

Reinicio de ciclo: Al volver a su base, todos los temporizadores y estados se limpian, dejando la plataforma lista para reaccionar ante una nueva colisión.
