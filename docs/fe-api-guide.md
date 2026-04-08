# HappyFurniture API — Hướng dẫn cho Frontend

> Base URL: `https://<your-domain>/api`  
> Auth: Bearer JWT trong header `Authorization: Bearer <token>` (chỉ cần cho các endpoint có ghi chú **[Admin]**)

---

## 1. Assembly (Kiểu lắp ráp)

Assembly là dữ liệu tham chiếu (reference data) — tương tự Category/Material. Mỗi Product có thể gán 1 assembly type.

### 1.1 Lấy danh sách Assembly (cho dropdown)

```
GET /api/assemblies/active
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Knock-down",
    "code": "KD",
    "description": "Sản phẩm cần tự lắp ráp",
    "isActive": true,
    "createdAt": "2026-04-06T16:31:21Z",
    "updatedAt": "2026-04-06T16:31:21Z"
  },
  {
    "id": 2,
    "name": "Fully Assembled",
    "code": "FA",
    "description": "Sản phẩm đã lắp ráp hoàn toàn",
    "isActive": true,
    "createdAt": "2026-04-06T16:31:21Z",
    "updatedAt": "2026-04-06T16:31:21Z"
  },
  {
    "id": 3,
    "name": "Semi Knock-down",
    "code": "SKD",
    "description": "Sản phẩm lắp ráp một phần",
    "isActive": true,
    "createdAt": "2026-04-06T16:31:21Z",
    "updatedAt": "2026-04-06T16:31:21Z"
  }
]
```

---

### 1.2 Lấy danh sách Assembly (có phân trang + filter)

```
GET /api/assemblies?pageNumber=1&pageSize=10&name=Knock&isActive=true
```

**Query params:**

| Param | Type | Mô tả |
|-------|------|--------|
| `pageNumber` | int | Trang hiện tại (default: 1) |
| `pageSize` | int | Số item/trang (default: 10, max: 50) |
| `name` | string? | Tìm theo tên (contains) |
| `isActive` | bool? | Lọc theo trạng thái |

**Response:**
```json
{
  "items": [ ... ],
  "totalCount": 3,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

---

### 1.3 Lấy Assembly theo ID

```
GET /api/assemblies/{id}
```

---

### 1.4 Tạo Assembly [Admin]

```
POST /api/assemblies
Content-Type: application/json
```

**Body:**
```json
{
  "name": "Knock-down",
  "code": "KD",
  "description": "Sản phẩm cần tự lắp ráp",
  "isActive": true
}
```

| Field | Required | Mô tả |
|-------|----------|--------|
| `name` | ✅ | Tên (max 255 ký tự) |
| `code` | ❌ | Mã viết tắt, VD: KD, SKD, FA (max 20 ký tự) |
| `description` | ❌ | Mô tả (max 1000 ký tự) |
| `isActive` | ❌ | Default: `true` |

---

### 1.5 Cập nhật Assembly [Admin]

```
PUT /api/assemblies/{id}
Content-Type: application/json
```

**Body:**
```json
{
  "name": "Knock-down",
  "code": "KD",
  "description": "Sản phẩm cần tự lắp ráp",
  "isActive": true
}
```

---

### 1.6 Xóa Assembly [Admin]

```
DELETE /api/assemblies/{id}
```

---

### 1.7 Seed data khuyến nghị

Gọi 3 lần POST `/api/assemblies` để tạo đủ 3 loại:

```json
{ "name": "Knock-down", "code": "KD", "description": "Sản phẩm cần tự lắp ráp hoàn toàn", "isActive": true }
{ "name": "Fully Assembled", "code": "FA", "description": "Sản phẩm đã được lắp ráp sẵn", "isActive": true }
{ "name": "Semi Knock-down", "code": "SKD", "description": "Sản phẩm lắp ráp một phần", "isActive": true }
```

---

## 2. Product — Assembly Filter & Field

### 2.1 Filter sản phẩm theo Assembly

Thêm `assemblyId` vào query string:

```
GET /api/products?assemblyId=1
GET /api/products?assemblyId=2&pageNumber=1&pageSize=12
```

### 2.2 Gán Assembly khi tạo/sửa Product

Trong body của `POST /api/products` hoặc `PUT /api/products/{id}`, thêm field `assemblyId`:

```json
{
  "name": "Ghế sofa ABC",
  "slug": "ghe-sofa-abc",
  "assemblyId": 1,
  "...": "..."
}
```

Để bỏ assembly, truyền `"assemblyId": null`.

### 2.3 Response Product có Assembly

Khi Product có assembly, response trả về object lồng nhau:

```json
{
  "id": 10,
  "name": "Ghế sofa ABC",
  "assemblyId": 1,
  "assembly": {
    "id": 1,
    "name": "Knock-down",
    "code": "KD",
    "description": "Sản phẩm cần tự lắp ráp",
    "isActive": true,
    "createdAt": "2026-04-06T16:31:21Z",
    "updatedAt": "2026-04-06T16:31:21Z"
  }
}
```

Nếu không có assembly: `"assemblyId": null, "assembly": null`.

---

## 3. Đa ngôn ngữ (i18n) — Category & Product

Tất cả field tiếng Anh đều **nullable** — không bắt buộc phải điền. Khi `null`, frontend dùng field tiếng Việt làm fallback.

---

### 3.1 Category — Fields đa ngôn ngữ

#### Tạo / Cập nhật Category

```
POST /api/categories
PUT  /api/categories/{id}
Content-Type: application/json
```

```json
{
  "name": "Phòng khách",
  "nameEn": "Living Room",
  "description": "Nội thất dành cho phòng khách",
  "descriptionEn": "Furniture for living room",
  "imageUrl": "https://...",
  "parentId": null,
  "sortOrder": 1,
  "isActive": true
}
```

#### Response Category

```json
{
  "id": 1,
  "name": "Phòng khách",
  "nameEn": "Living Room",
  "description": "Nội thất dành cho phòng khách",
  "descriptionEn": "Furniture for living room",
  "imageUrl": "https://...",
  "parentId": null,
  "sortOrder": 1,
  "isActive": true,
  "createdAt": "2026-04-06T16:38:45Z",
  "updatedAt": "2026-04-06T16:38:45Z",
  "parent": null,
  "children": []
}
```

---

### 3.2 Product — Fields đa ngôn ngữ

#### Tạo / Cập nhật Product

```
POST /api/products
PUT  /api/products/{id}
Content-Type: application/json
```

```json
{
  "name": "Ghế sofa ABC",
  "nameEn": "Sofa ABC",
  "slug": "ghe-sofa-abc",
  "description": "Ghế sofa cao cấp với thiết kế hiện đại",
  "descriptionEn": "Premium sofa with modern design",
  "detail": "<p>Chi tiết sản phẩm...</p>",
  "detailEn": "<p>Product detail...</p>",
  "deliveryInfo": "Giao hàng trong 3-5 ngày làm việc",
  "deliveryInfoEn": "Delivery within 3-5 business days",
  "assemblyId": 1,
  "isFeatured": false,
  "isActive": true,
  "categoryIds": [1, 2],
  "materialIds": [3],
  "imageUrls": [
    "https://cdn.example.com/products/sofa-1.jpg",
    "https://cdn.example.com/products/sofa-2.jpg"
  ]
}
```

`imageUrls` là input đơn giản để backend tạo/cập nhật các bản ghi trong `ProductImages` cho ảnh chung của product.
`images` trong response là dữ liệu đầy đủ lấy từ bảng `ProductImages` sau khi lưu.
Các ảnh gắn riêng cho variant nên quản lý qua endpoint `ProductImages` hoặc luồng variant riêng, không trộn vào `imageUrls`.

#### Bảng fields i18n

| Field VN | Field EN | Mô tả |
|----------|----------|-------|
| `name` | `nameEn` | Tên sản phẩm |
| `description` | `descriptionEn` | Mô tả ngắn |
| `detail` | `detailEn` | Chi tiết (hỗ trợ HTML) |
| `deliveryInfo` | `deliveryInfoEn` | Thông tin giao hàng |

#### Response Product (đầy đủ fields i18n)

```json
{
  "id": 10,
  "name": "Ghế sofa ABC",
  "nameEn": "Sofa ABC",
  "slug": "ghe-sofa-abc",
  "description": "Ghế sofa cao cấp với thiết kế hiện đại",
  "descriptionEn": "Premium sofa with modern design",
  "detail": "<p>Chi tiết sản phẩm...</p>",
  "detailEn": "<p>Product detail...</p>",
  "deliveryInfo": "Giao hàng trong 3-5 ngày làm việc",
  "deliveryInfoEn": "Delivery within 3-5 business days",
  "dimensionsHeight": 80,
  "dimensionsWidth": 200,
  "dimensionsDepth": 90,
  "dimensionUnit": "cm",
  "weight": 45.5,
  "isFeatured": false,
  "isActive": true,
  "assemblyId": 1,
  "assembly": {
    "id": 1,
    "name": "Knock-down",
    "code": "KD",
    "description": "Sản phẩm cần tự lắp ráp",
    "isActive": true
  },
  "categories": [],
  "materials": [],
  "variants": [],
  "images": [],
  "createdAt": "2026-04-06T16:38:45Z",
  "updatedAt": "2026-04-06T16:38:45Z"
}
```

---

## 4. Gợi ý xử lý i18n phía Frontend

```typescript
// Utility helper — dùng cho cả Category lẫn Product
function getLocalized(
  obj: Record<string, string | null | undefined>,
  field: string,
  locale: 'vi' | 'en'
): string {
  const enField = `${field}En`;
  if (locale === 'en' && obj[enField]) return obj[enField] as string;
  return (obj[field] as string) ?? '';
}

// Ví dụ
const name        = getLocalized(product, 'name', locale);
const description = getLocalized(product, 'description', locale);
const detail      = getLocalized(product, 'detail', locale);
const catName     = getLocalized(category, 'name', locale);
```

---

## 5. Tổng quan endpoints

### Assembly

| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| GET | `/api/assemblies` | ❌ | Danh sách có phân trang + filter |
| GET | `/api/assemblies/active` | ❌ | Tất cả active (dùng cho dropdown) |
| GET | `/api/assemblies/{id}` | ❌ | Chi tiết |
| POST | `/api/assemblies` | ✅ Admin | Tạo mới |
| PUT | `/api/assemblies/{id}` | ✅ Admin | Cập nhật |
| DELETE | `/api/assemblies/{id}` | ✅ Admin | Xóa |

### Category — fields mới

| Field | Kiểu | Nullable | Mô tả |
|-------|------|----------|-------|
| `nameEn` | string | ✅ | Tên tiếng Anh |
| `description` | string | ✅ | Mô tả tiếng Việt |
| `descriptionEn` | string | ✅ | Mô tả tiếng Anh |

### Product — fields mới

| Field | Kiểu | Nullable | Mô tả |
|-------|------|----------|-------|
| `nameEn` | string | ✅ | Tên tiếng Anh |
| `descriptionEn` | string | ✅ | Mô tả tiếng Anh |
| `detailEn` | string | ✅ | Chi tiết tiếng Anh |
| `deliveryInfoEn` | string | ✅ | Giao hàng tiếng Anh |
| `assemblyId` | int | ✅ | FK đến Assembly |
| `assembly` | object | ✅ | Nested object trong response |

### Product Filter — params mới

```
GET /api/products?assemblyId=1&categoryId=2&isActive=true&pageNumber=1&pageSize=12
```

| Param | Type | Mô tả |
|-------|------|--------|
| `assemblyId` | int? | Lọc theo kiểu lắp ráp |
