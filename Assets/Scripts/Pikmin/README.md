# 🌱 Scripts de Pikmin - Guía Educativa

¡Hola! Aquí aprenderás todo sobre los Pikmin - esas criaturas adorables que te ayudan en el juego.

---

## 📚 ¿Qué hay en esta carpeta?

Esta carpeta tiene **11 scripts** que hacen funcionar a los Pikmin:

### Scripts Principales:
1. **Pikmin.cs** - El cerebro básico de un Pikmin
2. **PikminManager.cs** - El jefe que organiza a TODOS los Pikmin
3. **PikminLauncher.cs** - Para lanzar Pikmin como una catapulta
4. **PikminOnion.cs** - La nave de donde nacen los Pikmin

### Scripts de Tipos de Pikmin:
5. **PikminType.cs** - La plantilla para todos los tipos
6. **RedPikmin.cs** - Pikmin rojo (resistente al fuego) 🔴
7. **BluePikmin.cs** - Pikmin azul (puede nadar) 💙
8. **YellowPikmin.cs** - Pikmin amarillo (resiste electricidad) ⚡
9. **WhitePikmin.cs** - Pikmin blanco (resiste veneno) ⚪
10. **DarkPikmin.cs** - Pikmin negro (ve en la oscuridad) ⚫

### Scripts de Habilidades:
11. **PikminCarrier.cs** - Hace que Pikmin carguen cosas
12. **PikminCombat.cs** - Hace que Pikmin peleen con enemigos

---

## 🧠 Pikmin.cs - ¡El cerebro de un Pikmin!

### ¿Qué hace?
Este es el script más importante. Hace que un Pikmin:
- Te siga como un patito siguiendo a su mamá 🦆
- Aterrice suavemente cuando lo lanzas
- Camine sin chocar con cosas
- Se una a una formación con otros Pikmin

### ¿Cómo funciona?

Imagina que un Pikmin es como un robot pequeño con diferentes "modos":

**Modo 1: En el aire** 🎈
- Acabas de lanzarlo
- Está volando por el aire
- Gira para mirar hacia donde va

**Modo 2: Aterrizando** 🪂
- Toca el suelo
- Frena un poquito (como cuando saltas y te detienes)
- Espera un momento antes de empezar a seguirte

**Modo 3: Siguiéndote** 🏃
- ¡Ya está listo!
- Te sigue a donde vayas
- Mantiene una distancia para no chocar contigo

### Partes importantes del código:

```csharp
void FollowPlayer()
{
    // Calcula hacia dónde ir
    Vector3 direction = targetPosition - transform.position;

    // Si está muy lejos, camina hacia ti
    if (distance > stopDistance)
    {
        // Muévete hacia el objetivo
        rb.velocity = direction * moveSpeed;
    }
}
```

**¿Qué significa?**
- `targetPosition` = donde debería estar el Pikmin (en la formación)
- `transform.position` = donde está ahora el Pikmin
- `direction` = la diferencia (hacia dónde caminar)
- Si está lejos de ti, camina hacia ti
- Si está cerca, ¡se queda quieto!

### Configuración en Unity:

**Move Speed** (Velocidad):
- 3 = Camina despacio (como una tortuga) 🐢
- 7 = Camina rápido (como un conejo) 🐰
- ¡Los Pikmin blancos son más rápidos!

**Follow Distance** (Distancia para Seguir):
- Qué tan cerca te siguen
- 2 = Cerca (formación apretada)
- 5 = Lejos (formación suelta)

**Stop Distance** (Distancia para Parar):
- Cuándo dejan de moverse
- 1 = Se paran muy cerca de ti
- 3 = Se paran más lejos

### 🎓 Experimento para Aprender:

1. Cambia Move Speed a 10 en un Pikmin
2. Cambia Move Speed a 1 en otro Pikmin
3. Lánzalos a los dos
4. ¿Cuál llega primero a ti?

---

## 👑 PikminManager.cs - ¡El jefe de los Pikmin!

### ¿Qué hace?
Es como un director de orquesta. Organiza a todos los Pikmin para que:
- Formen figuras bonitas (círculo, cuadrado, línea, triángulo)
- No se amontonen todos en el mismo lugar
- Sepan quién va primero y quién va después
- Respondan cuando los llamas con el silbato

### ¿Cómo funciona?

Imagina que tienes 20 Pikmin y quieres organizarlos:

**Sin Manager**:
- ❌ Todos se amontonan en el mismo lugar
- ❌ Se chocan entre ellos
- ❌ Es un desastre

**Con Manager**:
- ✅ Cada Pikmin tiene su lugar asignado
- ✅ Forman un círculo bonito alrededor tuyo
- ✅ Se mueven juntos como un equipo

### Formaciones:

**Círculo** 🔵:
```
       P  P  P
    P           P
   P      TÚ     P
    P           P
       P  P  P
```

**Cuadrado** ⬜:
```
P P P P P
P P P P P
P P TÚ P P
P P P P P
P P P P P
```

**Triángulo** 🔺:
```
      P
     P P
    P P P
   P P P P
  P P TÚ P P
```

**Línea** ➖:
```
TÚ - P - P - P - P - P
```

### Partes importantes del código:

```csharp
Vector3 GetCircleFormation(int index)
{
    // Calcula el ángulo para este Pikmin
    float angle = (index * 360f / totalPikmin);

    // Calcula la posición usando el ángulo
    float x = Cos(angle) * radius;
    float z = Sin(angle) * radius;

    return new Vector3(x, 0, z);
}
```

**¿Qué significa?**
- Imagina un reloj ⏰
- El primer Pikmin está en las 12
- El segundo en la 1
- El tercero en las 2
- Y así hasta formar un círculo completo

**¿Qué son Seno y Coseno?**
- Son funciones matemáticas mágicas
- Te ayudan a dibujar círculos
- `Cos(ángulo)` te da la posición X
- `Sin(ángulo)` te da la posición Z
- ¡Juntos hacen un círculo perfecto!

### Configuración en Unity:

**Max Pikmin** (Máximo de Pikmin):
- Cuántos Pikmin puedes tener
- 50 = Ejército pequeño
- 100 = ¡Ejército grande!
- 200 = ¡SÚPER EJÉRCITO!

**Formation Type** (Tipo de Formación):
- Circle = Círculo (clásico de Pikmin)
- Square = Cuadrado
- Triangle = Triángulo
- Line = Línea

**Formation Spacing** (Espacio en la Formación):
- Qué tan separados están
- 0.5 = Muy juntos (apretados)
- 2 = Muy separados (sueltos)

### Controles:

- **Presiona C** = Llama a todos los Pikmin cercanos con silbato
- **Presiona X** = Despide a todos (se van)
- **Presiona 1** = Formación de círculo
- **Presiona 2** = Formación de cuadrado
- **Presiona 3** = Formación de triángulo
- **Presiona 4** = Formación de línea

### 🎓 Experimento para Aprender:

1. Reúne 10 Pikmin
2. Presiona 1, 2, 3, 4 para cambiar formaciones
3. ¿Cuál formación te gusta más?
4. ¿Cuál es mejor para pasar por lugares estrechos?

---

## 🎯 PikminLauncher.cs - ¡La catapulta de Pikmin!

### ¿Qué hace?
Te permite lanzar Pikmin como si tuvieras una resortera. Ves una línea que muestra dónde va a caer el Pikmin.

### ¿Cómo funciona?

Es como lanzar una pelota de básquetbol:

1. **Apuntas** → Aparece una línea curva (trayectoria)
2. **Calculas** → ¿Llegará hasta allá?
3. **Lanzas** → ¡El Pikmin vuela por el aire!
4. **Aterriza** → El Pikmin cae donde apuntabas

### Física del lanzamiento:

```csharp
Vector3 CalculateLaunchVelocity(start, end, height)
{
    // Calcula cuánto tiempo estará en el aire
    float time = Sqrt(-2 * height / gravity);

    // Calcula la velocidad hacia arriba
    Vector3 velocityY = Up * Sqrt(-2 * gravity * height);

    // Calcula la velocidad horizontal
    Vector3 velocityXZ = (end - start) / time;

    // ¡Suma las dos velocidades!
    return velocityXZ + velocityY;
}
```

**¿Qué significa?**
- `height` = qué tan alto va el Pikmin en el arco
- `gravity` = la gravedad (lo que te jala hacia abajo)
- `velocityY` = velocidad hacia arriba
- `velocityXZ` = velocidad hacia adelante
- Es matemática de parábolas, ¡como en la clase de física!

### La trayectoria:

Imagina que lanzas una pelota:
```
     Inicio
       |
        \
         \    ← Sube
          •   ← Punto más alto
         /    ← Baja
        /
       |
     Fin
```

El script dibuja puntitos en cada parte del arco para que veas dónde va a caer.

### Configuración en Unity:

**Max Launch Distance** (Distancia Máxima):
- Qué tan lejos puedes lanzar
- 10 = Lanzamiento corto
- 20 = Lanzamiento medio
- 50 = ¡Súper lanzamiento!

**Arc Height** (Altura del Arco):
- Qué tan alto sube el Pikmin
- 2 = Arco bajo (rápido pero peligroso)
- 5 = Arco alto (lento pero seguro)
- 10 = ¡Muy alto! (tarda mucho en caer)

**Trajectory Resolution** (Resolución de Trayectoria):
- Cuántos puntitos tiene la línea
- 10 = Línea tosca
- 30 = Línea suave
- 50 = Línea súper suave

### 🎓 Experimento para Aprender:

1. Cambia Arc Height a 2 → Lanzamiento bajo
2. Cambia Arc Height a 10 → Lanzamiento alto
3. ¿Cuál es más fácil para apuntar?
4. ¿Cuál llega más rápido al suelo?

---

## 🏠 PikminOnion.cs - ¡La nave de los Pikmin!

### ¿Qué hace?
El Onion es como la casa de los Pikmin. Puede:
- Guardar Pikmin (como una alcancía)
- Crear nuevos Pikmin cuando le traes pellets
- Hacer que Pikmin salgan del suelo (nacimiento)
- Emerger del suelo cuando lo activas

### ¿Cómo funciona?

El Onion tiene tres estados (como un semáforo):

**🟤 Enterrado**:
- Está bajo tierra
- Esperando que lo actives
- No hace nada todavía

**🟡 Emergiendo**:
- Está saliendo del suelo
- Sube poco a poco
- Casi listo

**🟢 Activo**:
- ¡Ya está fuera!
- Puede crear Pikmin
- Puede recibir pellets

### Proceso de nacimiento de un Pikmin:

1. Le traes un pellet al Onion
2. El Onion lo absorbe (como comer)
3. El Onion guarda Pikmin en su "barriga"
4. Cuando quieres más Pikmin, salen del suelo:

```
   [Onion] ← Flota en el aire
      |
      | (semilla invisible cae)
      ↓
   _______ ← Suelo
      |
     \|/ ← Pikmin bajo tierra
      P
      ↑
      P  ← Pikmin excavando hacia arriba
      ↑
     [P] ← ¡Pikmin sale del suelo!
```

### Partes importantes del código:

```csharp
IEnumerator EmergeFromGround(pikmin, startPos, groundPos)
{
    // El Pikmin está bajo tierra
    while (underground)
    {
        // Muévelo hacia arriba poco a poco
        height += emergeSpeed * Time.deltaTime;
        pikmin.position = groundPos + Up * height;
    }

    // ¡Saltito final!
    // Pop hacia arriba
    // Luego baja al suelo
}
```

**¿Qué significa?**
- `IEnumerator` = Una función que tarda varios frames
- `while (underground)` = Mientras está bajo tierra, sigue subiendo
- `Time.deltaTime` = Un poquito cada frame
- Es como ver crecer una planta en cámara rápida

### Configuración en Unity:

**Max Pikmin In Onion** (Máximo en el Onion):
- Cuántos Pikmin puede guardar
- 50 = Onion pequeño
- 100 = Onion normal
- 200 = ¡Onion gigante!

**Current Pikmin Count** (Pikmin Actuales):
- Cuántos Pikmin hay guardados al empezar
- 5 = Empiezas con pocos
- 20 = Empiezas con un buen grupo
- 0 = ¡No tienes ninguno!

**Dig Depth** (Profundidad de Excavación):
- Qué tan profundo están los Pikmin bajo tierra
- 1 = Casi en la superficie
- 3 = Bien profundo
- 5 = ¡Muy profundo!

**Emerge Speed** (Velocidad de Emergencia):
- Qué tan rápido salen del suelo
- 1 = Salen lento (como zombies)
- 5 = Salen rápido
- 10 = ¡Salen disparados!

### 🎓 Experimento para Aprender:

1. Pon Current Pikmin Count en 20
2. Activa el Onion
3. Observa cómo los 20 Pikmin salen del suelo
4. ¿Se ven ordenados o desordenados?
5. Cambia Spawn Radius para que salgan más separados

---

## 🎨 Tipos de Pikmin - ¡Las habilidades especiales!

### PikminType.cs - La Plantilla Base

Este script es como una receta vacía. No hace mucho solo, pero todos los Pikmin de colores lo usan como base.

**¿Qué tiene?**
- Resistencias (a qué son inmunes)
- Multiplicadores (qué tan fuertes/rápidos son)
- Habilidades (qué pueden hacer)

### RedPikmin.cs - ¡Resistente al fuego! 🔴

**Superpoder**: No le hace daño el fuego

**¿Cómo funciona?**
```csharp
if (hazardType == "fire")
{
    return true; // ¡No me hace daño!
}
```

**Habilidad Extra**: Puede apagar fuegos
- Se acerca al fuego
- El fuego se hace más pequeño
- ¡Hasta que se apaga!

**Bonus de Combate**: Hace 1.5x más daño que otros Pikmin
- Pikmin normal = 10 de daño
- Pikmin rojo = 15 de daño
- ¡Perfecto para pelear!

### BluePikmin.cs - ¡Puede nadar! 💙

**Superpoder**: No se ahoga en el agua

**¿Cómo funciona?**
```csharp
if (isInWater)
{
    // Aplica flotación (sube hacia la superficie)
    rb.AddForce(Up * buoyancyForce);
}
```

**Habilidad Extra**: Puede rescatar a otros Pikmin
- Si un Pikmin normal cae al agua
- El Pikmin azul lo empuja hacia arriba
- ¡Lo salva de ahogarse!

**En el agua**:
- Pikmin normal: 😱 "¡Ayuda! ¡Me ahogo!"
- Pikmin azul: 😊 "¡Qué rico nadar!"

### YellowPikmin.cs - ¡Salta muy alto! ⚡

**Superpoder**: Resiste electricidad + salta muy alto

**¿Cómo funciona el salto?**
```csharp
void PerformHighJump()
{
    rb.AddForce(Up * jumpForce, ForceMode.Impulse);
}
```

**Habilidad Extra**: Destruye paredes eléctricas
- Se acerca a la pared
- La va destruyendo poco a poco
- ¡Abre nuevos caminos!

**Salto alto**:
- Pikmin normal salta: 1 metro
- Pikmin amarillo salta: 3 metros
- ¡Puede llegar a lugares altos!

### WhitePikmin.cs - ¡Detector de tesoros! ⚪

**Superpoder**: Resiste veneno + encuentra tesoros

**¿Cómo funciona?**
```csharp
void DetectBuriedTreasures()
{
    // Busca en un círculo alrededor del Pikmin
    Collider[] treasures = OverlapSphere(position, radius);

    // Si encuentra un tesoro
    if (tesoro != null && tesoro.IsHidden)
    {
        tesoro.Reveal(); // ¡Lo revela!
    }
}
```

**Habilidad Extra**: Es el más rápido
- Speed Multiplier = 1.5
- ¡Corre más rápido que todos!

**Bonus Tóxico**: Si un enemigo se lo come
- ¡El enemigo recibe daño de veneno!
- Es como comer algo podrido
- Defiende a tu equipo incluso al morir

### DarkPikmin.cs - ¡Ve en la oscuridad! ⚫

**Superpoder**: Camina en zonas oscuras sin daño

**¿Cómo funciona?**
```csharp
if (hazardType == "dark")
{
    // Puede llevar una luz
    if (light != null)
    {
        light.enabled = true; // Enciende la luz
    }
    return true; // No recibe daño
}
```

**Habilidad Extra**: Puede llevar una linterna
- Ilumina caminos oscuros
- Ayuda a otros Pikmin
- ¡Guía al equipo!

---

## 💪 PikminCarrier.cs - ¡Cargar objetos!

### ¿Qué hace?
Hace que los Pikmin trabajen en equipo para cargar cosas pesadas, como pellets o tesoros.

### ¿Cómo funciona?

Imagina que quieres mover un sofá:

**1 persona sola** = ❌ Muy pesado, no puede
**2 personas** = ⚠️ Casi, pero todavía difícil
**4 personas** = ✅ ¡Perfecto! Se puede mover

Lo mismo con Pikmin:

```
Pellet de Peso 5:
  P P P P P  ← 5 Pikmin cargando
     [🔴]    ← Pellet
  Caminando hacia el Onion →
```

### Partes importantes del código:

```csharp
if (carrierCount >= requiredCarriers)
{
    StartCarrying(); // ¡Hay suficientes Pikmin!

    // Mueve el objeto
    object.position = MoveTowards(currentPos, onionPos, speed);
}
```

**Sistema de Trabajo en Equipo:**

1. Pikmin 1 llega al pellet → Se adhiere, esperando
2. Pikmin 2 llega al pellet → Se adhiere, esperando
3. Pikmin 3 llega al pellet → ¡Hay suficientes! Empiezan a cargar
4. Los 3 Pikmin caminan juntos hacia el Onion
5. Llegan al Onion → El pellet se absorbe
6. ¡Los Pikmin quedan libres para otra tarea!

### Configuración:

**Detection Radius** (Radio de Detección):
- Qué tan cerca debe estar el Pikmin del objeto
- 2 = Debe estar muy cerca
- 5 = Puede estar más lejos

**Carry Speed** (Velocidad al Cargar):
- Qué tan rápido caminan cargando
- 2 = Lento (objeto muy pesado)
- 5 = Rápido (objeto ligero)

### 🎓 Experimento para Aprender:

1. Crea un pellet con Weight = 1
2. Crea un pellet con Weight = 10
3. ¿Cuántos Pikmin se necesitan para cada uno?
4. ¿Cuál llega más rápido al Onion?

---

## ⚔️ PikminCombat.cs - ¡A la batalla!

### ¿Qué hace?
Hace que los Pikmin ataquen a los enemigos. ¡Es hora de pelear!

### ¿Cómo funciona?

Un Pikmin en batalla tiene varios pasos:

**Paso 1: Detectar** 👀
- El Pikmin ve un enemigo cerca
- Decide atacar

**Paso 2: Acercarse** 🏃
- Corre hacia el enemigo
- Se prepara para el ataque

**Paso 3: ¡Atacar!** 💥
- Se adhiere al enemigo (como un imán)
- Empieza a golpear
- Hace daño cada segundo

**Paso 4: ¡Aguantar!** 💪
- El enemigo intenta sacudirlo
- El Pikmin resiste
- Si el enemigo sacude muy fuerte, ¡el Pikmin sale volando!

### Sistema de Adherencia (Latch):

```
     🦖 ← Enemigo
    P P P ← Pikmin adheridos
```

Mientras están adheridos:
- Atacan cada 1 segundo
- El enemigo pierde vida
- Si el enemigo sacude, algunos se caen

### Partes importantes del código:

```csharp
void PerformAttack()
{
    // Calcula el daño
    float damage = attackDamage * strengthMultiplier;

    // ¡Golpea al enemigo!
    enemy.TakeDamage(damage);

    // Espera antes del próximo ataque
    WaitForSeconds(attackInterval);
}
```

**Sistema Anti-Sacudida:**
```csharp
void OnShakenOff(float shakeForce)
{
    // ¿Resiste la sacudida?
    if (Random.value > shakeOffResistance)
    {
        // ¡Salió volando!
        rb.AddForce(awayFromEnemy * shakeForce);
    }
    else
    {
        // ¡Aguantó!
        // Sigue adherido
    }
}
```

### Configuración:

**Attack Damage** (Daño de Ataque):
- Cuánto daño hace cada golpe
- 5 = Débil
- 10 = Normal
- 20 = ¡Fuerte!

**Attack Interval** (Intervalo de Ataque):
- Cada cuánto tiempo ataca
- 0.5 = Ataca muy rápido
- 1 = Ataca normal
- 2 = Ataca lento

**Latch Duration** (Duración Adherido):
- Cuánto tiempo puede quedarse adherido
- 3 segundos = Se cae rápido
- 10 segundos = Aguanta mucho
- 99 segundos = ¡Casi nunca se cae!

**Shake Off Resistance** (Resistencia a Sacudidas):
- Qué tan bien resiste las sacudidas
- 0.3 = Se cae fácil
- 0.7 = Resiste bien
- 0.9 = ¡Casi imposible de sacudir!

### Matemática de combate:

**Ejemplo**: 5 Pikmin atacan a un enemigo de 100 HP

```
Cada Pikmin hace 10 de daño cada 1 segundo
5 Pikmin × 10 daño = 50 daño por segundo
100 HP ÷ 50 daño/segundo = 2 segundos para matarlo
```

**Pero con Pikmin rojos:**
```
Bonus de 1.5x
5 Pikmin × 10 daño × 1.5 = 75 daño por segundo
100 HP ÷ 75 daño/segundo = 1.33 segundos
¡Más rápido!
```

### 🎓 Experimento para Aprender:

1. Pon Attack Damage en 100 → ¡Súper fuerte!
2. Pon Attack Interval en 0.1 → ¡Ataca rapidísimo!
3. Pelea contra un enemigo
4. ¿Es muy fácil? ¿Muy difícil?
5. Encuentra el balance perfecto

---

## 🎯 ¿Cómo trabajan todos juntos?

Imagina una misión completa:

### Misión: Derrotar un enemigo y llevar su cuerpo al Onion

**Paso 1: Organización** (PikminManager)
- Tienes 10 Pikmin en formación de círculo
- Están siguiéndote

**Paso 2: Lanzamiento** (PikminLauncher + Pikmin)
- Ves un enemigo
- Lanzas 5 Pikmin hacia él
- Los Pikmin aterrizan cerca del enemigo

**Paso 3: Combate** (PikminCombat + PikminType)
- Los 5 Pikmin atacan al enemigo
- 3 Pikmin rojos (hacen más daño)
- 2 Pikmin azules (normal)
- Se adhieren al enemigo

**Paso 4: Victoria** (EnemyCombat)
- El enemigo muere
- Se convierte en un "cadáver-pellet"

**Paso 5: Transporte** (PikminCarrier)
- Los 5 Pikmin se adhieren al cadáver
- Lo cargan hacia el Onion
- Caminan en formación

**Paso 6: Recompensa** (PikminOnion)
- Llegan al Onion
- El Onion absorbe el cadáver
- Crea 5 Pikmin nuevos
- Los nuevos Pikmin salen del suelo

**¡Resultado!**: Empezaste con 10 Pikmin, ¡ahora tienes 15!

---

## 🧪 Proyectos para Practicar

### Proyecto 1: Ejército de Velocidad
1. Crea 5 Pikmin blancos (rápidos)
2. Crea 5 Pikmin normales
3. Mándalos a todos a un punto lejano
4. ¿Quiénes llegan primero?
5. **Aprenderás**: Cómo la velocidad afecta el juego

### Proyecto 2: Formaciones Creativas
1. Abre PikminManager
2. Modifica el código de formación para crear:
   - Una estrella ⭐
   - Un corazón ❤️
   - Tu nombre (con puntos)
3. **Aprenderás**: Matemáticas de posiciones

### Proyecto 3: Laboratorio de Combate
1. Crea un enemigo con 500 HP
2. Manda 1 Pikmin → Cuenta cuánto tarda
3. Manda 5 Pikmin → Cuenta cuánto tarda
4. Manda 10 Pikmin → Cuenta cuánto tarda
5. **Aprenderás**: Trabajo en equipo = más eficiente

### Proyecto 4: La Cadena de Producción
1. Pon 3 pellets en el mapa
2. Programa que 3 grupos de Pikmin los carguen
3. Observa cómo se multiplican tus Pikmin
4. **Aprenderás**: Estrategia y recursos

---

## 📖 Palabras Importantes (Glosario)

**Formación**: Cómo se organizan los Pikmin alrededor tuyo

**Trayectoria**: El camino curvo que hace algo cuando lo lanzas

**Parábola**: La forma de arco que hace la trayectoria

**Adherir/Latch**: Pegarse a algo (como los Pikmin a los enemigos)

**Resistencia**: No recibir daño de algo (fuego, agua, etc.)

**Multiplicador**: Un número que hace algo más grande (×2 = el doble)

**Corrutina (Coroutine)**: Una función que tarda varios frames

**Seno y Coseno**: Funciones matemáticas para hacer círculos

**Frame**: Un cuadro del juego (60 por segundo)

**Component**: Una pieza que le da habilidades a un objeto

---

## 🎉 ¡Felicidades!

Ahora entiendes cómo funcionan los Pikmin. Son criaturas pequeñas pero con sistemas muy inteligentes:

- **Cerebro individual** (Pikmin.cs) - Cada uno piensa por sí mismo
- **Cerebro colectivo** (PikminManager.cs) - Trabajan como equipo
- **Habilidades únicas** (PikminType) - Cada color es especial
- **Trabajo en equipo** (PikminCarrier) - Juntos son más fuertes
- **Valentía** (PikminCombat) - Pelean para protegerte

Los Pikmin son el corazón del juego. ¡Sin ellos, no habría juego!

**Recuerda**: Cada Pikmin es como un pequeño robot con su propia IA (Inteligencia Artificial). Programar IA es una de las partes más divertidas de hacer videojuegos.

**Sigue aprendiendo y experimentando.** ¡Tal vez algún día programes tus propios personajes con IA! 🤖🌟
