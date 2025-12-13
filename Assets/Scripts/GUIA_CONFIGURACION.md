# 🎮 Clon de Pikmin - Guía Completa de Configuración

## 📋 Tabla de Contenidos
1. [Configuración del Proyecto Unity](#configuración-del-proyecto-unity)
2. [Etiquetas y Capas](#etiquetas-y-capas)
3. [Configuración del Jugador](#configuración-del-jugador)
4. [Configuración de Pikmin](#configuración-de-pikmin)
5. [Configuración de Enemigos](#configuración-de-enemigos)
6. [Configuración de Obstáculos](#configuración-de-obstáculos)
7. [Configuración del Onion](#configuración-del-onion)
8. [Configuración de Pellets](#configuración-de-pellets)
9. [Lista de Verificación](#lista-de-verificación)

---

## Configuración del Proyecto Unity

### Requisitos
- **Versión de Unity:** 6.0 o posterior
- **Pipeline de Renderizado:** URP o Built-in (el código soporta ambos)
- **Sistema de Input:** Legacy Input (usa Input.GetKey, Input.GetAxis)

### Configuración Inicial de la Escena
1. Crea una nueva escena
2. Agrega un **Plane** para el suelo (escala 10x1x10 o más grande)
3. Agrega una **Directional Light**
4. Asegúrate de que existe la Main Camera

---

## Etiquetas y Capas

### Etiquetas Requeridas
Ve a **Edit > Project Settings > Tags and Layers**

#### Etiquetas a Crear:
1. `Player` - para el personaje del jugador
2. `Fire` - para peligros de fuego
3. `Water` - para peligros de agua
4. `Poison` - para peligros de veneno
5. `Electric` - para peligros eléctricos

### Capas Requeridas
Crea estas capas (el número exacto no importa):

1. **Ground** - para suelo/terreno
2. **Pikmin** - para personajes Pikmin
3. **Enemy** - para enemigos
4. **Obstacle** - para obstáculos que Pikmin deben evitar
5. **Carryable** - para objetos que Pikmin pueden cargar
6. **Hazard** - para todos los peligros

---

## Configuración del Jugador

### 1. Crear GameObject del Jugador
1. Crea un **Empty GameObject** llamado "Player"
2. Agrega la etiqueta "Player"
3. Posiciona en (0, 1, 0)

### 2. Agregar Visual (Modelo del Personaje)
1. Agrega una **Cápsula** como hijo (Escala: 0.5, 2, 0.5)
2. O importa tu propio modelo 3D

### 3. Agregar Componentes Requeridos

#### a) Collider
- Agrega **Capsule Collider**
  - Radius: 0.5
  - Height: 2
  - Center: (0, 1, 0)

#### b) Rigidbody
- Agrega **Rigidbody**
  - Mass: 1
  - Drag: 0
  - Angular Drag: 0.05
  - Use Gravity: ✓
  - Is Kinematic: ✗
  - Constraints: Freeze Rotation X, Y, Z ✓

#### c) Componente Health
- Agrega **Scripts/Player/Health.cs**
  - Max Health: 100
  - Destroy On Death: ✗

#### d) Player Controller
- Agrega **Scripts/Player/PlayerController.cs**
  - Move Speed: 5
  - Sprint Speed: 8
  - Ground Layer: Selecciona "Ground"
  - Camera Transform: Encontrará automáticamente la Main Camera

#### e) Whistle Controller
- Agrega **Scripts/Player/WhistleController.cs**
  - Whistle Key: Mouse1 (Click Derecho)
  - Min Radius: 1
  - Max Radius: 10
  - Ground Layer: Selecciona "Ground"
  - Pikmin Layer: Selecciona "Pikmin"

### 4. Configurar la Cámara

#### Opción A: Usar la Main Camera Existente
1. Selecciona Main Camera
2. Agrega **Scripts/Player/CameraController.cs**
3. Configura:
   - Target: Arrastra el GameObject Player
   - Offset: (0, 15, -10)
   - Follow Speed: 5
   - Allow Rotation: ✓
   - Rotate Left Key: Q
   - Rotate Right Key: E

#### Opción B: Crear Nueva Cámara
1. Crea un nuevo GameObject Camera
2. Etiquétalo como "MainCamera"
3. Sigue los pasos anteriores

---

## Configuración de Pikmin

### 1. Crear Prefab Base de Pikmin

#### a) Crear GameObject
1. Crea un **Empty GameObject** llamado "Pikmin_Rojo"
2. Agrega la capa "Pikmin"

#### b) Agregar Visual
1. Agrega una **Cápsula** como hijo
   - Escala: (0.3, 0.5, 0.3)
   - Posición: (0, 0.5, 0)
2. Crea un material, establece el color a Rojo

#### c) Agregar Componentes

**Física:**
- **Rigidbody**
  - Mass: 0.5
  - Drag: 0
  - Angular Drag: 0.05
  - Use Gravity: ✓
  - Freeze Rotation: X, Y, Z ✓

- **Capsule Collider**
  - Radius: 0.3
  - Height: 1
  - Center: (0, 0.5, 0)

**Scripts Principales:**
- **Scripts/Player/Health.cs**
  - Max Health: 50
  - Destroy On Death: ✓
  - Destroy Delay: 0.5

- **Scripts/Pikmin/Pikmin.cs**
  - Move Speed: 5
  - Follow Distance: 2
  - Ground Layer: "Ground"
  - Obstacle Layer: "Obstacle"

- **Scripts/Pikmin/RedPikmin.cs**
  - Fire Layer: "Hazard"

- **Scripts/Pikmin/PikminCarrier.cs**
  - Detection Radius: 2
  - Carryable Layer: "Carryable"

- **Scripts/Pikmin/PikminCombat.cs**
  - Attack Damage: 5
  - Detection Radius: 5
  - Enemy Layer: "Enemy"

### 2. Crear Prefab
1. Arrastra "Pikmin_Rojo" a la carpeta **Assets/Prefabs/**
2. Elimina de la escena

### 3. Crear Otros Tipos de Pikmin
Duplica el prefab de Pikmin Rojo y modifica:

**Pikmin Azul:**
- Cambia el color a Azul
- Reemplaza `RedPikmin.cs` con `BluePikmin.cs`
- Water Layer: "Hazard"

**Pikmin Amarillo:**
- Cambia el color a Amarillo
- Reemplaza con `YellowPikmin.cs`
- Electric Layer: "Hazard"

**Pikmin Blanco:**
- Cambia el color a Blanco
- Reemplaza con `WhitePikmin.cs`
- Poison Layer: "Hazard"
- Speed Multiplier: 1.5

---

## Configuración de Enemigos

### 1. Crear Enemigo Básico

#### a) Crear GameObject
1. Crea un **Cubo** llamado "Enemigo_Basico"
2. Agrega la capa "Enemy"
3. Escala: (2, 2, 2)
4. Crea un material rojo

#### b) Agregar Componentes

- **Rigidbody**
  - Mass: 5
  - Use Gravity: ✓
  - Freeze Rotation: X, Y, Z ✓

- **Box Collider**
  - Size: (2, 2, 2)

- **Scripts/Player/Health.cs**
  - Max Health: 100
  - Destroy On Death: ✓

- **Scripts/EnemyMovement.cs**
  - Player: Arrastra el GameObject Player
  - Move Speed: 3
  - Stopping Distance: 1

- **Scripts/EnemyCombat.cs**
  - Attack Damage: 10
  - Attack Range: 2
  - Target Layers: Selecciona "Pikmin" y "Player"
  - Corpse Weight: 5
  - Corpse Value: 5

### 2. Crear Prefab del Enemigo
1. Arrastra a la carpeta **Assets/Prefabs/**
2. Elimina de la escena

---

## Configuración de Obstáculos

### Ejemplo de Peligro de Fuego

1. Crea un **Cubo** llamado "PeligroFuego"
2. Escala: (3, 1, 3)
3. Agrega la capa "Hazard"
4. Etiqueta como "Fire"

**Componentes:**
- **Box Collider**
  - Is Trigger: ✓

- **Scripts/Obstacles/FireHazard.cs**
  - Damage Per Second: 10
  - Fire Color: Naranja
  - Respawn After Extinguish: ✓

**Visual Opcional:**
- Agrega **Particle System** (partículas de fuego)
- Agrega **Light** (naranja, intensidad 2)

### Ejemplo de Peligro de Agua

1. Crea un **Cubo** llamado "PeligroAgua"
2. Escala: (5, 0.5, 5)
3. Position.y: 0
4. Agrega la capa "Hazard"
5. Etiqueta como "Water"

**Componentes:**
- **Box Collider**
  - Is Trigger: ✓

- **Scripts/Obstacles/WaterHazard.cs**
  - Drowning Damage: 20
  - Water Color: Azul con alpha 0.6

### Ejemplo de Pared Eléctrica

1. Crea un **Cubo** llamado "ParedElectrica"
2. Escala: (1, 3, 3)
3. Agrega la capa "Hazard"
4. Etiqueta como "Electric"

**Componentes:**
- **Box Collider**
  - Is Trigger: ✓

- **Scripts/Obstacles/ElectricWall.cs**
  - Damage Per Second: 15
  - Electric Color: Amarillo

---

## Configuración del Onion

### 1. Crear GameObject del Onion

1. Crea una **Esfera** llamada "Onion_Rojo"
2. Escala: (2, 2, 2)
3. Posición: (10, 3, 0) - empieza enterrado

**Visual:**
- Crea un material rojo
- Agrega efecto de brillo (emisión)

### 2. Agregar Componentes

- **Sphere Collider**
  - Radius: 1
  - Is Trigger: ✓

- **Scripts/Pikmin/PikminOnion.cs**
  - Pikmin Prefab: Arrastra el prefab "Pikmin_Rojo"
  - Max Pikmin In Onion: 50
  - Current Pikmin Count: 5 (cantidad inicial)
  - Spawn Point: Crea un GameObject hijo vacío en (0, -2, 0)
  - Dig Depth: 2
  - Ground Layer: "Ground"
  - Start Deactivated: ✓ (empieza enterrado)
  - Buried Depth: 3
  - Require Player Touch: ✓

### 3. Crear Objetos Hijos

**Punto de Aparición:**
1. Crea un hijo vacío "SpawnPoint"
2. Posición: (0, -2, 0)
3. Asigna al campo "Spawn Point" del Onion

**Punto de Recepción de Pellets:**
1. Crea un hijo vacío "PelletReceivePoint"
2. Posición: (0, 2, 0)
3. Asigna al campo "Pellet Receive Point" del Onion

---

## Configuración de Pellets

### 1. Crear Pellet

1. Crea una **Esfera** llamada "Pellet_1"
2. Escala: (0.8, 0.8, 0.8)
3. Agrega la capa "Carryable"

**Visual:**
- Crea un material (color que coincida con el tipo de Pikmin)
- Agrega TextMesh para el número "1"

### 2. Agregar Componentes

- **Rigidbody**
  - Mass: 1
  - Use Gravity: ✓

- **Sphere Collider**
  - Radius: 0.4
  - Is Trigger: ✗

- **Scripts/Pellet.cs**
  - Pikmin Value: 1
  - Weight: 1
  - Pellet Number: 1
  - Pellet Type: Number

### 3. Crear Variantes de Números
Duplica y cambia:
- Pellet_5: Valor 5, Peso 3
- Pellet_10: Valor 10, Peso 5
- Pellet_20: Valor 20, Peso 10

---

## Configuración del PikminManager

### 1. Crear GameObject Manager

1. Crea un **Empty GameObject** llamado "PikminManager"
2. Posición: (0, 0, 0)

### 2. Agregar Componente

- **Scripts/Pikmin/PikminManager.cs**
  - Player Transform: Arrastra el Player
  - Max Pikmin: 100
  - Formation Type: Circle
  - Formation Spacing: 1
  - Whistle Key: C
  - Dismiss Key: X

---

## Lista de Verificación

### Movimiento Básico
- [ ] El jugador se mueve con WASD
- [ ] El jugador rota hacia el cursor del mouse
- [ ] La cámara sigue al jugador
- [ ] La cámara rota con Q/E

### Aparición de Pikmin
- [ ] Camina hacia el Onion para activarlo
- [ ] El Onion emerge del suelo
- [ ] Los Pikmin aparecen desde bajo tierra
- [ ] Los Pikmin emergen con animación de excavación

### Pikmin Siguiendo
- [ ] Los Pikmin siguen al jugador en formación
- [ ] Mantén Click Derecho para abrir el silbato
- [ ] El círculo del silbato crece mientras lo mantienes
- [ ] Suelta para llamar a los Pikmin en el radio
- [ ] Los Pikmin llamados se unen a la formación

### Lanzamiento de Pikmin
- [ ] Mantén Click Izquierdo para apuntar
- [ ] Aparece la línea de trayectoria
- [ ] Suelta para lanzar el Pikmin
- [ ] El Pikmin vuela por el aire
- [ ] El Pikmin aterriza y sigue

### Pikmin Cargando
- [ ] Los Pikmin detectan pellets cercanos
- [ ] Múltiples Pikmin se adhieren al pellet
- [ ] El pellet comienza a moverse cuando hay suficientes Pikmin
- [ ] El pellet es entregado al Onion
- [ ] El Onion absorbe el pellet
- [ ] Nuevos Pikmin se agregan al almacenamiento

### Combate de Pikmin
- [ ] Los Pikmin detectan enemigos
- [ ] Los Pikmin se acercan a los enemigos
- [ ] Los Pikmin se sujetan a los enemigos
- [ ] Los Pikmin infligen daño con el tiempo
- [ ] La vida del enemigo disminuye
- [ ] El enemigo muere cuando vida = 0
- [ ] El enemigo se convierte en cadáver
- [ ] Los Pikmin pueden cargar el cadáver

### Comportamiento del Enemigo
- [ ] El enemigo se mueve hacia el jugador
- [ ] El enemigo ataca al jugador
- [ ] El enemigo ataca a los Pikmin
- [ ] El enemigo come Pikmin (el Pikmin desaparece)
- [ ] El enemigo sacude a los Pikmin adheridos
- [ ] El enemigo muere por ataques de Pikmin

### Peligros
- [ ] El fuego daña a los Pikmin que no son Rojos
- [ ] Los Pikmin Rojos son inmunes al fuego
- [ ] Los Pikmin Rojos extinguen el fuego
- [ ] El agua ahoga a los Pikmin que no son Azules
- [ ] Los Pikmin Azules nadan en el agua
- [ ] Las paredes eléctricas dañan a los Pikmin que no son Amarillos
- [ ] Los Pikmin Amarillos destruyen paredes eléctricas

### UI/Retroalimentación
- [ ] El círculo del silbato es visible
- [ ] El círculo del silbato cambia de color al llamar Pikmin
- [ ] La barra de vida del jugador se actualiza
- [ ] El conteo de Pikmin se muestra correctamente

---

## Problemas Comunes

### Los Pikmin No Siguen
**Solución:**
- Verifica que el Player tenga la etiqueta "Player"
- Verifica que Ground Layer esté configurado correctamente
- Asegúrate de que PikminManager existe en la escena

### El Silbato No Funciona
**Solución:**
- Verifica que la Camera esté asignada
- Verifica Ground Layer para raycasting
- Verifica que Pikmin Layer esté configurado

### Los Pikmin No Atacan
**Solución:**
- Verifica que Enemy Layer esté configurado
- Asegúrate de que el enemigo tenga el componente Health
- Verifica el radio de detección de PikminCombat

### Cargar No Funciona
**Solución:**
- Verifica que Carryable Layer esté configurado
- Asegúrate de que el pellet tenga Rigidbody
- Verifica el radio de detección de PikminCarrier

---

## Próximos Pasos

### Adiciones de Fase 2
1. Efectos de sonido
2. Sistema de UI/HUD
3. Ciclo día/noche
4. Sistema de guardar/cargar

### Contenido Adicional
1. Más tipos de Pikmin (Morado, Roca, Alado)
2. Más tipos de enemigos con IA variada
3. Enemigos jefes
4. Más tipos de peligros
5. Construcción de puentes
6. Destrucción de puertas

---

## 🎯 Flujo de Trabajo de Inicio Rápido

**Configuración Mínima Viable (10 minutos):**

1. **Etiquetas/Capas** (2 min)
   - Crear: Etiquetas Player, Fire, Water, Poison, Electric
   - Crear: Capas Ground, Pikmin, Enemy, Carryable

2. **Jugador** (3 min)
   - Empty GameObject + visual de Cápsula
   - Agregar: Rigidbody, Collider, Health, PlayerController
   - Cámara: Agregar CameraController

3. **Pikmin** (3 min)
   - GameObject Cápsula (pequeño)
   - Agregar: Rigidbody, Collider, Health, Pikmin, RedPikmin, PikminCarrier, PikminCombat
   - Crear prefab

4. **Manager** (1 min)
   - Empty GameObject
   - Agregar: PikminManager
   - Asignar referencia del jugador

5. **Prueba** (1 min)
   - Coloca un prefab de Pikmin en la escena
   - Modo Play
   - Muévete con WASD
   - Click derecho para silbato
   - ¡Llama al Pikmin!

---

## 📞 Soporte

### Si Algo No Funciona:
1. Revisa la sección **Problemas Comunes**
2. Activa `showDebugInfo` en los componentes para ver logs
3. Revisa la Consola de Unity para mensajes de error
4. Verifica que las etiquetas y capas estén configuradas correctamente
5. Asegúrate de que todos los componentes requeridos estén adjuntos

### Ubicaciones de Archivos:
- **Jugador:** `Assets/Scripts/Player/`
- **Pikmin:** `Assets/Scripts/Pikmin/`
- **Enemigos:** `Assets/Scripts/` (EnemyMovement.cs, EnemyCombat.cs)
- **Obstáculos:** `Assets/Scripts/Obstacles/`
- **Docs:** `Assets/Scripts/` (este archivo, SETUP_GUIDE.md)

---

## 💡 Consejos de Uso

### Para Empezar:
1. Lee esta guía completamente antes de comenzar
2. Crea etiquetas y capas PRIMERO
3. Configura el jugador con todos los componentes
4. Crea un prefab de Pikmin
5. Prueba movimiento y silbato
6. Agrega el Onion a la escena
7. Prueba el ciclo completo de juego

### Mejores Prácticas:
- **Siempre prueba incrementalmente** - agrega un sistema a la vez
- **Verifica capas y etiquetas** - la mayoría de problemas vienen de aquí
- **Usa modo Debug** - activa showDebugInfo para solucionar problemas
- **Empieza simple** - haz que las mecánicas básicas funcionen antes de agregar complejidad

### Errores Comunes a Evitar:
- ❌ Olvidar configurar Ground Layer
- ❌ No etiquetar al jugador como "Player"
- ❌ Falta de Rigidbody en Pikmin
- ❌ No crear prefabs antes de probar
- ❌ Olvidar asignar PikminManager

---

## 🎉 Controles del Juego

### Jugador
- **WASD** - Moverse
- **Shift Izquierdo** - Correr
- **Mouse** - Rotar hacia el cursor
- **Q/E** - Rotar cámara

### Pikmin
- **Click Izquierdo (mantener)** - Apuntar y lanzar Pikmin
- **Click Derecho (mantener)** - Abrir silbato
- **Click Derecho (soltar)** - Llamar Pikmin en el círculo
- **C** - Llamar todos los Pikmin cercanos
- **X** - Despedir todos los Pikmin

### Formaciones
- **1** - Formación Circular
- **2** - Formación Cuadrada
- **3** - Formación Triangular
- **4** - Formación en Línea

---

**🎉 ¡Listo para jugar! ¡Diviértete con tu clon de Pikmin!** 🌱
