# 📊 Archivos Excel de Prueba - Importación Masiva

## ✅ Archivos Creados

### 📦 **productos_test.xlsx** (14 productos)
Archivo Excel con datos **desnormalizados correctamente**.

**Estructura:**
- **Una sola fila de encabezados** con columnas duplicadas:
  - `ProductName` | `Nombre` | `Price` | `Precio` | `Stock` | `Description` | `Descripción`
- **14 filas de datos** donde:
  - Algunos productos usan columnas en inglés (`ProductName`, `Price`, `Description`)
  - Otros usan columnas en español (`Nombre`, `Precio`, `Descripción`)
  - Las columnas no usadas quedan vacías

**Productos incluidos:**
- Laptop Dell XPS 15 - $1,200.50
- Mouse Logitech MX Master 3 - $89.99
- Monitor LG 27 4K - $450.00
- Teclado, RAM, SSD, Webcam, Cables, Auriculares, Tablet, etc.

### 👥 **clientes_test.xlsx** (13 clientes)
Archivo Excel con datos **desnormalizados correctamente**.

**Estructura:**
- **Una sola fila de encabezados** con columnas duplicadas:
  - `FullName` | `NombreCompleto` | `Email` | `Correo` | `Phone` | `Teléfono` | `Address` | `Dirección` | `Document` | `Documento`
- **13 filas de datos** mezclando columnas en español e inglés

**Clientes incluidos:**
- Juan Pérez García - juan.perez@email.com
- María López Martínez - maria.lopez@email.com
- Carlos Rodríguez - carlos.r@email.com
- + 10 clientes más

## 🎯 Cómo Usar

1. **Abre** la aplicación web (http://localhost:4200)
2. **Inicia sesión** como Admin
3. **Ve a Products** o **Users**
4. **Click** en "Importar Excel"
5. **Selecciona** `productos_test.xlsx` o `clientes_test.xlsx`
6. **Revisa** los resultados de la importación

## ✨ Qué Probará

El sistema debe:
- ✅ Leer la fila de header y detectar TODAS las columnas
- ✅ Normalizar datos leyendo de columnas en inglés O español
- ✅ Importar todos los 14 productos exitosamente
- ✅ Importar todos los 13 clientes exitosamente

**Resultado esperado:** 100% de éxito en la importación.
