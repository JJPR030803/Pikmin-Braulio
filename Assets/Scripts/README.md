# 🎮 Braulio's Pikmin Game - Guía Educativa Completa

¡Bienvenido a tu juego de Pikmin! Esta guía te enseñará CÓMO FUNCIONA TODO en el juego. 🌟

---

## 🗺️ Mapa de Aprendizaje

Esta guía tiene diferentes secciones. ¡Empieza por donde quieras!

```
📚 GUÍAS EDUCATIVAS
│
├── 👤 Player/ (Jugador)
│   ├── PlayerController.cs - Cómo te mueves
│   ├── CameraController.cs - Cómo la cámara te sigue
│   ├── WhistleController.cs - Cómo llamas a los Pikmin
│   └── Health.cs - Tu vida
│   📖 → Lee: Player/README.md
│
├── 🌱 Pikmin/ (Tus ayudantes)
│   ├── Pikmin.cs - Base de todos los Pikmin
│   ├── PikminManager.cs - Organiza a todos
│   ├── PikminLauncher.cs - Lanza Pikmin
│   ├── PikminFormation.cs - Formaciones (círculo, cuadrado)
│   ├── PikminCarrier.cs - Cargar objetos
│   ├── PikminCombat.cs - Pelear con enemigos
│   ├── Tipos específicos:
│   │   ├── RedPikmin.cs - Resiste fuego 🔥
│   │   ├── BluePikmin.cs - Resiste agua 💧
│   │   ├── YellowPikmin.cs - Resiste electricidad ⚡
│   │   └── WhitePikmin.cs - Resiste veneno ☠️
│   └── Onion.cs - De donde nacen los Pikmin
│   📖 → Lee: Pikmin/README.md
│
├── 👹 Enemy/ (Enemigos)
│   ├── EnemyMovement.cs - Cómo te persiguen
│   └── EnemyCombat.cs - Cómo atacan
│   📖 → Lee: Enemy/README.md
│
├── ⚠️ Obstacles/ (Obstáculos y Peligros)
│   ├── ObstacleBase.cs - Base de todos los obstáculos
│   ├── FireHazard.cs - Fuego que quema 🔥
│   ├── WaterHazard.cs - Agua que ahoga 💧
│   ├── ElectricWall.cs - Electricidad que electrocuta ⚡
│   └── PoisonGas.cs - Gas venenoso ☠️
│   📖 → Lee: Obstacles/README.md
│
└── 📦 Otros (Objetos importantes)
    ├── Pellet.cs - Objetos que cargan los Pikmin
    ├── BuriedTreasure.cs - Tesoros enterrados
    └── Health.cs - Sistema de vida
```

---

## 🎯 ¿Por Dónde Empezar?

### Si eres NUEVO en programación:
1. **Empieza aquí** ⬇️ (sigue leyendo esta guía)
2. Luego ve a **Player/README.md** (aprende a moverte)
3. Después **Pikmin/README.md** (aprende sobre tus ayudantes)
4. Luego **Enemy/README.md** (aprende sobre enemigos)
5. Por último **Obstacles/README.md** (aprende sobre peligros)

### Si YA SABES programar un poco:
- Ve directo a la sección que te interesa
- Cada README está completo y se puede leer solo

### Si quieres CREAR algo específico:
- **Crear un nuevo tipo de Pikmin**: Lee Pikmin/README.md → Sección "Tipos de Pikmin"
- **Crear un nuevo enemigo**: Lee Enemy/README.md → Sección "Proyectos"
- **Crear un nuevo obstáculo**: Lee Obstacles/README.md → Sección "Herencia"
- **Cambiar controles**: Lee Player/README.md → Sección "PlayerController"

---

## 🧩 ¿Cómo Funciona el Juego? (Vista General)

### El Ciclo del Juego

Imagina que el juego es como armar un rompecabezas gigante. Cada pieza hace algo diferente:

```
┌─────────────────────────────────────────────────┐
│                   EL JUGADOR                    │
│  Tú controlas al capitán con WASD y el mouse   │
└─────────────────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────┐
│              LOS PIKMIN (Ayudantes)             │
│  • Silbas para llamarlos (Click Derecho)       │
│  • Los lanzas a enemigos/objetos (Click Izq)   │
│  • Te siguen en formación                       │
└─────────────────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────┐
│         LAS ACCIONES (Lo que hacen)             │
│  • Atacar enemigos → Obtener cadáveres         │
│  • Cargar pellets → Llevar al Onion            │
│  • Romper obstáculos → Abrir caminos           │
└─────────────────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────┐
│           RECOMPENSAS (Lo que ganas)            │
│  • Más Pikmin nacen del Onion                  │
│  • Tu ejército crece                            │
│  • ¡Puedes derrotar enemigos más grandes!      │
└─────────────────────────────────────────────────┘
```

### Una Partida Típica (Ejemplo Real):

**Minuto 1:**
- Empiezas con 5 Pikmin Rojos
- Encuentras un Onion
- ¡Nacen 10 Pikmin más!
- Ahora tienes 15 Pikmin

**Minuto 3:**
- Ves un pellet pequeño
- Lanzas 3 Pikmin
- Cargan el pellet al Onion
- ¡Nacen 5 Pikmin más!
- Ahora tienes 20 Pikmin

**Minuto 5:**
- Encuentras un enemigo
- Lanzas 15 Pikmin
- El enemigo se sacude
- Vuelves a lanzar más Pikmin
- ¡El enemigo muere!
- Se convierte en pellet grande (peso 10)

**Minuto 7:**
- 10 Pikmin cargan el cadáver del enemigo
- Lo llevan al Onion
- ¡Nacen 15 Pikmin más!
- Ahora tienes 35 Pikmin

**Minuto 10:**
- Encuentras un muro de fuego 🔥
- Tus Pikmin Rojos pueden atravesarlo
- Encuentras un tesoro del otro lado
- ¡Victoria!

---

## 🎮 Controles del Juego

### Teclado y Mouse

| Tecla/Acción | Qué Hace | Script Responsable |
|--------------|----------|-------------------|
| **W** | Caminar hacia adelante | PlayerController.cs |
| **A** | Caminar a la izquierda | PlayerController.cs |
| **S** | Caminar hacia atrás | PlayerController.cs |
| **D** | Caminar a la derecha | PlayerController.cs |
| **Left Shift** | Correr (sprint) | PlayerController.cs |
| **Mouse** | Rotar hacia el cursor | PlayerController.cs |
| **Click Izquierdo** | Lanzar Pikmin | PikminLauncher.cs |
| **Click Derecho** | Silbato (llamar Pikmin) | WhistleController.cs |
| **Q** | Rotar cámara izquierda | CameraController.cs |
| **E** | Rotar cámara derecha | CameraController.cs |
| **Rueda del Mouse** | Zoom in/out | CameraController.cs |

---

## 🧠 Conceptos Importantes que Aprenderás

Esta guía te enseña programación de verdad. Aquí hay algunos conceptos que encontrarás:

### 1. **Movimiento y Física**
- Cómo hacer que cosas se muevan
- Cómo hacer que choquen
- Gravedad y velocidad
- 📖 Aprende en: Player/README.md, Enemy/README.md

### 2. **Matemáticas del Juego**
- Círculos (formaciones de Pikmin)
- Parábolas (lanzar Pikmin)
- Distancias (qué tan lejos está algo)
- Ángulos (rotaciones)
- 📖 Aprende en: Pikmin/README.md

### 3. **Organización de Código**
- Clases base (ObstacleBase)
- Herencia (RedPikmin hereda de PikminType)
- Componentes (cada script es una pieza)
- 📖 Aprende en: Obstacles/README.md, Pikmin/README.md

### 4. **Inteligencia Artificial (AI)**
- Cómo los enemigos te persiguen
- Cómo los Pikmin siguen órdenes
- Cómo eligen qué hacer
- 📖 Aprende en: Enemy/README.md, Pikmin/README.md

### 5. **Listas y Colecciones**
- Guardar muchos Pikmin en una lista
- Contar cuántos hay
- Encontrar el más cercano
- 📖 Aprende en: Pikmin/README.md

### 6. **Eventos y Comunicación**
- Cuando algo muere, avisar a otros
- Cuando recoges algo, actualizar el contador
- Sistema de mensajes entre scripts
- 📖 Aprende en: Player/README.md (Health)

---

## 🏗️ Arquitectura del Juego (Cómo está Construido)

### Sistema de Componentes

Unity usa un sistema de "componentes". Cada objeto en el juego es como LEGO:

```
🧱 JUGADOR (GameObject)
├── 📦 Transform (posición, rotación, tamaño)
├── 📦 Rigidbody (física, gravedad)
├── 📦 Collider (para chocar con cosas)
├── 📦 PlayerController (movimiento)
├── 📦 WhistleController (silbato)
└── 📦 Health (vida)

🌱 PIKMIN (GameObject)
├── 📦 Transform
├── 📦 Rigidbody
├── 📦 Collider
├── 📦 Pikmin (comportamiento base)
├── 📦 RedPikmin (tipo específico)
├── 📦 PikminCombat (ataque)
├── 📦 PikminCarrier (cargar objetos)
└── 📦 Health (vida)

👹 ENEMIGO (GameObject)
├── 📦 Transform
├── 📦 Rigidbody
├── 📦 Collider
├── 📦 EnemyMovement (perseguir)
├── 📦 EnemyCombat (atacar)
└── 📦 Health (vida)
```

**¿Por qué componentes?**
- Puedes mezclar y combinar
- Reutilizar código
- Fácil de organizar
- Como LEGO: ¡construye lo que quieras!

### Sistema de Managers (Organizadores)

Algunos scripts "organizan" a otros:

**PikminManager.cs**
- Sabe cuántos Pikmin tienes
- Sabe dónde está cada uno
- Organiza las formaciones
- Es como el "jefe" de los Pikmin

**CarrierManager.cs**
- Organiza Pikmin que cargan un objeto
- Asegura que todos vayan en la misma dirección
- Cuenta cuántos se necesitan

---

## 🎨 Tipos de Pikmin y Sus Habilidades

| Tipo | Color | Habilidad Especial | Inmune a | Script |
|------|-------|-------------------|----------|--------|
| 🔴 Rojo | Rojo | Ataque fuerte (1.5x daño) | Fuego 🔥 | RedPikmin.cs |
| 🔵 Azul | Azul | Nada en agua | Agua 💧 | BluePikmin.cs |
| 🟡 Amarillo | Amarillo | Alcance de lanzamiento | Electricidad ⚡ | YellowPikmin.cs |
| ⚪ Blanco | Blanco | Veneno a enemigos | Veneno ☠️ | WhitePikmin.cs |

**Ejemplo de uso:**
- Hay un muro de fuego → Usa Pikmin Rojos
- Hay agua profunda → Usa Pikmin Azules
- Hay una pared eléctrica → Usa Pikmin Amarillos
- Enemigo muy fuerte → Deja que coma Pikmin Blancos (se envenena)

---

## ⚠️ Tipos de Obstáculos

| Obstáculo | Daño | ¿Quién es inmune? | Script |
|-----------|------|-------------------|--------|
| 🔥 Fuego | 15/seg | Pikmin Rojos | FireHazard.cs |
| 💧 Agua | 10/seg | Pikmin Azules | WaterHazard.cs |
| ⚡ Electricidad | 25/seg | Pikmin Amarillos | ElectricWall.cs |
| ☠️ Veneno | 20/seg | Pikmin Blancos | PoisonGas.cs |

**Estrategia:**
- Identifica el obstáculo
- Cambia al tipo correcto de Pikmin
- ¡Atraviesa sin peligro!

---

## 🎓 Proyectos de Aprendizaje (Para Practicar)

### Proyecto Nivel 1: "Mi Primer Nivel"
**Objetivo:** Crear un nivel simple para aprender lo básico

**Pasos:**
1. Crea un suelo (plano grande)
2. Agrega al jugador con PlayerController
3. Agrega un Onion
4. Agrega 5 Pikmin Rojos
5. Agrega un pellet pequeño (peso 1)
6. ¡Juega! Recoge el pellet

**¿Qué aprendes?**
- Movimiento del jugador
- Silbato
- Lanzar Pikmin
- Sistema de cargar objetos

---

### Proyecto Nivel 2: "Mi Primer Combate"
**Objetivo:** Derrotar un enemigo

**Pasos:**
1. Usa el nivel del Proyecto 1
2. Agrega un cubo rojo (enemigo)
3. Agrégale EnemyMovement + EnemyCombat + Health
4. Configuración del enemigo:
   - Max Health: 50
   - Move Speed: 3
   - Attack Damage: 10
5. ¡Derrótalo con tus Pikmin!

**¿Qué aprendes?**
- Combate con Pikmin
- Los enemigos contraatacan
- Sistema de vida
- Recompensas (pellet del cadáver)

---

### Proyecto Nivel 3: "Tipos de Pikmin"
**Objetivo:** Usar diferentes tipos

**Pasos:**
1. Crea 3 caminos separados
2. Camino 1: Muro de fuego → Tesoro
3. Camino 2: Charco de agua → Tesoro
4. Camino 3: Pared eléctrica → Tesoro
5. Crea 3 Onions (Rojo, Azul, Amarillo)
6. ¡Rescata los 3 tesoros!

**¿Qué aprendes?**
- Inmunidades de tipos
- Cambiar entre grupos
- Estrategia de tipos

---

### Proyecto Nivel 4: "El Jefe Final"
**Objetivo:** Crear y derrotar un jefe

**Pasos:**
1. Crea un enemigo GIGANTE (escala 5, 5, 5)
2. Configuración:
   - Health: 300
   - Attack Damage: 50
   - Corpse Weight: 30
   - Corpse Value: 100
3. Necesitarás MUCHOS Pikmin
4. Usa diferentes tipos
5. ¡Derrota al jefe!

**¿Qué aprendes?**
- Peleas largas
- Administrar recursos (Pikmin)
- Trabajo en equipo
- Recompensas grandes

---

## 🐛 Solución de Problemas Comunes

### "Mis Pikmin no me siguen"
**Solución:**
1. ¿Silbaste? (Click derecho)
2. ¿PikminManager está en la escena?
3. ¿El jugador tiene tag "Player"?
→ Lee: Pikmin/README.md → Sección "PikminManager"

### "El enemigo no me persigue"
**Solución:**
1. ¿Tiene EnemyMovement?
2. ¿Asignaste al jugador en "Player"?
3. ¿El move speed es mayor que 0?
→ Lee: Enemy/README.md → Sección "EnemyMovement"

### "Los Pikmin no pueden cargar objetos"
**Solución:**
1. ¿El objeto tiene Pellet component?
2. ¿Pusiste el weight correcto?
3. ¿Hay suficientes Pikmin? (si weight = 5, necesitas 5)
→ Lee: Pikmin/README.md → Sección "PikminCarrier"

### "La cámara no sigue al jugador"
**Solución:**
1. ¿CameraController está en la cámara?
2. ¿Asignaste al jugador en "Target"?
3. ¿El follow speed es mayor que 0?
→ Lee: Player/README.md → Sección "CameraController"

---

## 📚 Recursos Adicionales

### Documentos de Configuración
- **GUIA_CONFIGURACION.md** - Cómo configurar todo desde cero
- **SETUP_GUIDE.md** - Setup guide (versión en inglés)
- **NEW_FEATURES.md** - Lista de características implementadas
- **FIXES_APPLIED.md** - Bugs arreglados y compatibilidad Unity 6

### Guías Detalladas por Sistema
- **Player/README.md** - Sistema del jugador completo
- **Pikmin/README.md** - Sistema de Pikmin completo
- **Enemy/README.md** - Sistema de enemigos completo
- **Obstacles/README.md** - Sistema de obstáculos completo

---

## 🎯 Objetivos de Aprendizaje

Al terminar de leer todas las guías, sabrás:

### Programación Básica ✅
- Variables (números que guardan información)
- Condicionales (if, else - tomar decisiones)
- Bucles (for, foreach - repetir acciones)
- Funciones (pedazos de código reutilizables)

### Programación Orientada a Objetos ✅
- Clases (plantillas de objetos)
- Herencia (clases que heredan de otras)
- Componentes (piezas modulares)
- Eventos (sistema de mensajes)

### Desarrollo de Juegos ✅
- Física (movimiento, colisiones)
- Inteligencia Artificial (enemigos, Pikmin)
- Administración de recursos (Pikmin, vida)
- Diseño de niveles

### Matemáticas del Juego ✅
- Vectores (posiciones en 3D)
- Distancias (qué tan lejos está algo)
- Ángulos (rotaciones)
- Trigonometría básica (seno, coseno para círculos)

---

## 💡 Consejos para Aprender Mejor

### 1. **Lee, Prueba, Modifica**
- Lee la guía
- Prueba el código en Unity
- Cambia números y ve qué pasa
- ¡Romper cosas enseña mucho!

### 2. **Empieza Simple**
- No intentes hacer todo a la vez
- Haz un enemigo simple antes que un jefe
- Domina movimiento antes de agregar combate

### 3. **Experimenta**
- Todas las guías tienen sección "Experimentos"
- ¡Pruébalos todos!
- Inventa tus propios experimentos

### 4. **Pregunta "¿Por qué?"**
- ¿Por qué este número es 5 y no 10?
- ¿Por qué uso este componente?
- Entender el "por qué" es más importante que memorizar

### 5. **Crea Tus Propias Cosas**
- Modifica los scripts
- Crea nuevos tipos de Pikmin
- Inventa nuevos enemigos
- ¡Sé creativo!

---

## 🌟 Características Especiales del Juego

### Sistema de Onion
- Los Pikmin nacen de aquí
- Absorbe pellets para crear más Pikmin
- Diferentes colores para diferentes tipos
- Animación de emergencia desde el suelo
📖 Lee más en: Pikmin/README.md

### Sistema de Formaciones
- Los Pikmin se organizan en formas
- Círculo, cuadrado, triángulo, línea
- Usa matemáticas (trigonometría)
- ¡Se ve muy cool!
📖 Lee más en: Pikmin/README.md

### Sistema de Lanzamiento
- Trayectoria parabólica (como lanzar una pelota)
- Usa física real
- Diferentes ángulos y fuerzas
📖 Lee más en: Pikmin/README.md

### Sistema de Combate
- Los Pikmin se trepan a los enemigos
- Los enemigos se sacuden
- Daño por tipo
- ¡Como en el juego real!
📖 Lee más en: Pikmin/README.md, Enemy/README.md

---

## 🎉 ¡Empecemos!

Ahora que tienes el mapa completo, elige tu camino:

👤 **Quiero aprender sobre el jugador** → Ve a `Player/README.md`

🌱 **Quiero aprender sobre Pikmin** → Ve a `Pikmin/README.md`

👹 **Quiero aprender sobre enemigos** → Ve a `Enemy/README.md`

⚠️ **Quiero aprender sobre obstáculos** → Ve a `Obstacles/README.md`

🔧 **Quiero configurar el juego** → Ve a `GUIA_CONFIGURACION.md`

---

## 📞 Recuerda

- **No hay preguntas tontas** - Si no entiendes algo, está bien
- **Equivocarse es aprender** - Los errores enseñan
- **Practica mucho** - Mientras más hagas, mejor serás
- **Diviértete** - ¡Estás haciendo un videojuego de verdad!

---

## 🏆 Cuando Termines

Habrás aprendido:
- ✅ Cómo funciona un juego de verdad
- ✅ Programación orientada a objetos
- ✅ Física de juegos
- ✅ Inteligencia artificial básica
- ✅ Matemáticas prácticas
- ✅ Unity y C#

**¡Estos conocimientos sirven para crear CUALQUIER juego!** 🎮

Ya sea que quieras hacer:
- Un juego de carreras 🏎️
- Un juego de plataformas 🦘
- Un juego de estrategia 🏰
- ¡Lo que imagines!

Los conceptos son los mismos. Pikmin es una excelente manera de aprenderlos.

---

**¡Que comience la aventura! 🌟**

*Hecho con ❤️ para ayudarte a aprender programación de videojuegos*
