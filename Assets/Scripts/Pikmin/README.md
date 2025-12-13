# 🌱 Carpeta de Pikmin

¡Aquí está todo lo relacionado con los Pikmin! Estos son los pequeños ayudantes que te siguen en el juego.

## 📜 Scripts principales

### **Pikmin.cs** - El Pikmin básico
Este es el script principal que hace que los Pikmin funcionen.

**¿Qué hace?**
- Hace que el Pikmin te siga cuando lo lanzas
- Le dice cómo caminar y girar
- Hace que aterrice correctamente cuando lo lanzas
- Evita que choque con otros objetos

**Configuración en Unity:**
1. Arrastra el script `Pikmin.cs` a tu objeto de Pikmin
2. Asegúrate de que el Pikmin tenga un **Rigidbody** (para que tenga física)
3. Asegúrate de que tenga un **Collider** (para que choque con cosas)
4. Configura estos valores:
   - **Move Speed**: Qué tan rápido corre (ejemplo: 5)
   - **Follow Distance**: Qué tan cerca te sigue (ejemplo: 2)
   - **Ground Layer**: Marca qué es el suelo

---

### **PikminManager.cs** - El jefe de los Pikmin
Este script controla a TODOS los Pikmin al mismo tiempo.

**¿Qué hace?**
- Organiza a los Pikmin en formaciones (círculo, cuadrado, línea, triángulo)
- Cuenta cuántos Pikmin tienes
- Te permite llamar a todos los Pikmin con un silbato
- Controla que no tengas demasiados Pikmin

**Configuración en Unity:**
1. Crea un objeto vacío llamado "PikminManager"
2. Arrastra el script `PikminManager.cs` al objeto
3. Configura:
   - **Player Transform**: Arrastra al jugador aquí
   - **Max Pikmin**: Máximo de Pikmin permitidos (ejemplo: 100)
   - **Formation Type**: Elige la forma (Circle, Square, Triangle, Line)

**Controles:**
- Presiona **X** para despedir a todos los Pikmin
- Presiona **C** para llamar a los Pikmin con el silbato
- Presiona **1, 2, 3, 4** para cambiar la formación

---

### **PikminLauncher.cs** - Lanzador de Pikmin
Este script te permite lanzar Pikmin a donde apuntes con el mouse.

**¿Qué hace?**
- Te muestra una línea de trayectoria (donde va a caer el Pikmin)
- Lanza el Pikmin cuando sueltas el click
- Te dice si llegaste al límite de Pikmin

**Configuración en Unity:**
1. Arrastra el script al jugador o a un objeto "Launcher"
2. Configura:
   - **Pikmin Prefab**: Arrastra el prefab del Pikmin que quieres lanzar
   - **Launch Point**: Crea un objeto vacío enfrente del jugador y arrástralo aquí
   - **Ground Layer**: Selecciona la capa del suelo
   - **Trajectory Line**: Arrastra un componente **LineRenderer** para ver la trayectoria
   - **Max Launch Distance**: Qué tan lejos puedes lanzar (ejemplo: 15)

**Controles:**
- **Click izquierdo**: Apuntar y lanzar
- **Rueda del mouse**: Ajustar la fuerza del lanzamiento
- **Click derecho**: Cancelar el lanzamiento

---

## 🎨 Tipos de Pikmin

### **PikminType.cs** - Script base
Este es el script que usan todos los tipos de Pikmin. No lo uses directamente, usa los scripts específicos de cada color.

### **RedPikmin.cs** - Pikmin Rojo 🔴
**Habilidad especial:** ¡Resiste el fuego!

**Configuración:**
1. Arrastra `RedPikmin.cs` a tu Pikmin rojo
2. Configura:
   - **Fire Extinguish Radius**: Qué tan cerca debe estar del fuego para apagarlo (ejemplo: 2)
   - **Fire Layer**: Selecciona la capa de objetos de fuego

**¿Qué hace especial?**
- No le hace daño el fuego
- Puede apagar fuegos caminando cerca
- Es más fuerte atacando (1.5x de daño)

### **BluePikmin.cs** - Pikmin Azul 💙
**Habilidad especial:** ¡Puede nadar!

**Configuración:**
1. Arrastra `BluePikmin.cs` a tu Pikmin azul
2. No necesita configuración especial

**¿Qué hace especial?**
- Puede caminar bajo el agua
- No se ahoga en zonas de agua

### **YellowPikmin.cs** - Pikmin Amarillo ⚡
**Habilidad especial:** ¡Resiste electricidad!

**Configuración:**
1. Arrastra `YellowPikmin.cs` a tu Pikmin amarillo
2. Configura **Electric Layer** si tienes objetos eléctricos

**¿Qué hace especial?**
- No le hace daño la electricidad
- Puede romper paredes eléctricas
- Vuela más alto cuando lo lanzas

### **WhitePikmin.cs** - Pikmin Blanco ☠️
**Habilidad especial:** ¡Resiste veneno!

**Configuración:**
1. Arrastra `WhitePikmin.cs` a tu Pikmin blanco
2. Configura **Poison Layer** si tienes objetos venenosos

**¿Qué hace especial?**
- No le hace daño el veneno
- Puede caminar en zonas venenosas
- Es más rápido que otros Pikmin

### **DarkPikmin.cs** - Pikmin Negro 🌑
**Habilidad especial:** ¡Ve en la oscuridad!

**Configuración:**
1. Arrastra `DarkPikmin.cs` a tu Pikmin negro
2. Opcionalmente agrega un **Light** para que ilumine

**¿Qué hace especial?**
- Puede caminar en zonas oscuras sin daño
- Puede llevar una luz para iluminar

---

### **PikminOnion.cs** - La nave de los Pikmin
Este script controla la "Onion" (cebolla), que es donde nacen los Pikmin.

**¿Qué hace?**
- Crea nuevos Pikmin cuando le traes pellets
- Guarda los Pikmin cuando no los usas
- Cambia de color según el tipo de Pikmin

**Configuración:**
1. Crea un objeto 3D (como una esfera) para la Onion
2. Arrastra el script `PikminOnion.cs`
3. Configura:
   - **Pikmin Prefab**: El Pikmin que va a crear
   - **Onion Type**: El color de la Onion (Red, Blue, Yellow, etc.)
   - **Storage Capacity**: Cuántos Pikmin puede guardar (ejemplo: 100)

---

## ✅ Checklist para configurar Pikmin

- [ ] Crear un objeto con modelo 3D para el Pikmin
- [ ] Agregar componente **Rigidbody**
- [ ] Agregar componente **Capsule Collider** o **Sphere Collider**
- [ ] Agregar el script base **Pikmin.cs**
- [ ] Agregar el script de tipo (RedPikmin, BluePikmin, etc.)
- [ ] Configurar el tag del jugador como "Player"
- [ ] Crear una capa de suelo (Ground Layer)
- [ ] Convertir el Pikmin en un **Prefab** (arrástralo a la carpeta Prefabs)
- [ ] Poner el prefab en el **PikminLauncher**

## 🎮 ¿Cómo funciona todo junto?

1. El **PikminLauncher** crea un nuevo Pikmin cuando haces click
2. El Pikmin vuela por el aire usando su **Rigidbody**
3. Cuando aterriza, el **Pikmin.cs** hace que empiece a seguirte
4. El **PikminManager** organiza a todos los Pikmin en formación
5. Cada tipo de Pikmin (Rojo, Azul, etc.) tiene habilidades especiales

## 💡 Problemas comunes

**El Pikmin no se mueve:**
- Verifica que tenga un Rigidbody
- Verifica que el jugador tenga el tag "Player"
- Asegúrate de que "Use Gravity" esté activado en el Rigidbody

**El Pikmin atraviesa el suelo:**
- Verifica que el suelo tenga un Collider
- Configura correctamente el "Ground Layer"

**No puedo lanzar Pikmin:**
- Verifica que el PikminLauncher tenga el Pikmin Prefab asignado
- Verifica que el Launch Point esté configurado
- Asegúrate de tener un LineRenderer si quieres ver la trayectoria

---

**¡Ya estás listo para crear tu ejército de Pikmin!** 🌱✨
