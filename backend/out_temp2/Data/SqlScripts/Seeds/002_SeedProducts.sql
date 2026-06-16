-- 002_SeedProducts.sql

-- Auto-generated realistic seed products script

START TRANSACTION;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('93219693-f321-454c-b4d5-364ea76c2c0a', 'SP001', '8933953767242', 'Tiêu đen xay DH Foods hũ 50g', 'Tiêu đen xay DH Foods hũ 50g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 22000, 15000, 'https://picsum.photos/seed/SP001/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('6a3baa42-0349-448a-ba70-c8e6ed44eb2c', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '93219693-f321-454c-b4d5-364ea76c2c0a', 83, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('8b3b6b1a-fb00-4ee0-9e11-2f77a800ed70', 'SP002', '8938496965328', 'Đường cát trắng Biên Hòa túi 1kg', 'Đường cát trắng Biên Hòa túi 1kg chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 26000, 20000, 'https://picsum.photos/seed/SP002/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('2fd69f7c-08d2-47dd-af45-81a79676b8d1', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '8b3b6b1a-fb00-4ee0-9e11-2f77a800ed70', 146, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('dc8f403d-c964-42a6-876c-02a37f545e41', 'SP003', '8930122691669', 'Thạch dừa Minh Châu cốc 150g', 'Thạch dừa Minh Châu cốc 150g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 6000, 4000, 'https://picsum.photos/seed/SP003/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('2aec01fa-dc8b-4675-bfca-b179ad3e47a4', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'dc8f403d-c964-42a6-876c-02a37f545e41', 139, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('f4379dbf-664c-4df9-a5be-b50ccb02539b', 'SP004', '8934801845146', 'Sữa tươi tiệt trùng Vinamilk 180ml', 'Sữa tươi tiệt trùng Vinamilk 180ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 8500, 6500, 'https://picsum.photos/seed/SP004/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('82e6c221-51e6-4e89-aa43-a8f9b56ffd2b', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'f4379dbf-664c-4df9-a5be-b50ccb02539b', 60, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('8b3f831c-381b-49c7-bfe4-5d9f07d69039', 'SP005', '8930482814893', 'Kem chống nắng Skin Aqua tuyết hồng 50g', 'Kem chống nắng Skin Aqua tuyết hồng 50g chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 155000, 120000, 'https://picsum.photos/seed/SP005/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('a2500b03-1e26-4da5-b2da-70fd4814d572', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '8b3f831c-381b-49c7-bfe4-5d9f07d69039', 59, 10, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('c502b6e6-ee98-4f22-afb5-e53b17d1b420', 'SP006', '8932880957015', 'Xịt côn trùng Raid hương chanh 600ml', 'Xịt côn trùng Raid hương chanh 600ml chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 68000, 50000, 'https://picsum.photos/seed/SP006/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('8be1f04f-70d5-45ce-a45d-7094e9843f95', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'c502b6e6-ee98-4f22-afb5-e53b17d1b420', 98, 8, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('fb12cea1-fbfa-49c4-950b-21c4905a6919', 'SP007', '8930391171822', 'Pepsi 320ml', 'Pepsi 320ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 10000, 7000, 'https://picsum.photos/seed/SP007/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('2b4af438-082d-4e3d-aa60-74fb1b0c54fd', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'fb12cea1-fbfa-49c4-950b-21c4905a6919', 141, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('db226b91-0493-425a-8756-11bd75dc6acc', 'SP008', '8932489638346', 'Tập giấy kiểm tra cấp 2 Hồng Hà 20 tờ', 'Tập giấy kiểm tra cấp 2 Hồng Hà 20 tờ chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 7000, 4800, 'https://picsum.photos/seed/SP008/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('261f09f7-e8be-4e35-8e46-452ec9bec25c', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'db226b91-0493-425a-8756-11bd75dc6acc', 115, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('ca224612-d302-41ae-92ab-5d2751754397', 'SP009', '8938713315098', 'Bánh quy Cosy Kinh Đô 240g', 'Bánh quy Cosy Kinh Đô 240g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 25000, 19000, 'https://picsum.photos/seed/SP009/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('b7bec375-de4d-4efa-b17d-21523b210188', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'ca224612-d302-41ae-92ab-5d2751754397', 78, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('eef538e7-7542-4748-af5b-f3e6541fe11e', 'SP010', '8933010310518', 'Kẹp quần áo nhựa Song Long vỉ 12 cái', 'Kẹp quần áo nhựa Song Long vỉ 12 cái chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 18000, 12500, 'https://picsum.photos/seed/SP010/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('8ea8c145-bfa0-4f35-a74f-74f7ecbc336d', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'eef538e7-7542-4748-af5b-f3e6541fe11e', 80, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('ab806427-0596-40ad-ba3b-7d2a135e80e2', 'SP011', '8937382997376', 'Ruột chì kim Pentel 0.5mm tuýp', 'Ruột chì kim Pentel 0.5mm tuýp chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 10000, 7000, 'https://picsum.photos/seed/SP011/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('cc492d17-fa7e-46f8-a360-ecfc156a2265', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'ab806427-0596-40ad-ba3b-7d2a135e80e2', 68, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('1405de6a-999b-4897-8fa2-9f9ba5981257', 'SP012', '8931656670106', 'Trà sữa đất Việt Chupa Chups 200ml', 'Trà sữa đất Việt Chupa Chups 200ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 15000, 11000, 'https://picsum.photos/seed/SP012/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('03881977-fbb2-483a-b93a-91fa5e32e477', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '1405de6a-999b-4897-8fa2-9f9ba5981257', 106, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('1e6dd52d-0b78-4927-80a7-88df054b4221', 'SP013', '8933338726247', 'Dầu gội Clear Bạc Hà Mát Lạnh 650g', 'Dầu gội Clear Bạc Hà Mát Lạnh 650g chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 165000, 130000, 'https://picsum.photos/seed/SP013/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('d2b514b3-f205-43a9-a03a-df5d7819372f', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '1e6dd52d-0b78-4927-80a7-88df054b4221', 83, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('b12a2be9-fb15-4605-ad6c-0e4ddd25bb63', 'SP014', '8937810801326', 'Sữa tắm Lifebuoy Bảo Vệ Vượt Trội 850g', 'Sữa tắm Lifebuoy Bảo Vệ Vượt Trội 850g chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 175000, 140000, 'https://picsum.photos/seed/SP014/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('3cf0e03a-a17b-41d7-99ca-f3fddb9be6d4', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'b12a2be9-fb15-4605-ad6c-0e4ddd25bb63', 144, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('53bbb056-0f79-40c6-903f-c416eb216ef1', 'SP015', '8933602606474', 'Tẩy chì Pentel Standard nhỏ', 'Tẩy chì Pentel Standard nhỏ chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 6000, 4000, 'https://picsum.photos/seed/SP015/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('11312b29-d278-4696-a283-54f195927e34', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '53bbb056-0f79-40c6-903f-c416eb216ef1', 128, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('2378c39b-3c7f-490c-a75e-534a3575ce68', 'SP016', '8937234309805', 'Nước lau sàn Sunlight Hương Hoa 1kg', 'Nước lau sàn Sunlight Hương Hoa 1kg chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 32000, 23000, 'https://picsum.photos/seed/SP016/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('a47253f2-623c-4f87-9f78-e33cb107015f', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '2378c39b-3c7f-490c-a75e-534a3575ce68', 34, 5, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('77b3701b-8d90-41b9-a7f3-5a4118b9683d', 'SP017', '8939788208121', 'Nước xả vải Comfort Ban Mai túi 1.8L', 'Nước xả vải Comfort Ban Mai túi 1.8L chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 145000, 115000, 'https://picsum.photos/seed/SP017/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('4d477069-86ca-4f65-88d4-41fcb5a9a92c', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '77b3701b-8d90-41b9-a7f3-5a4118b9683d', 37, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('09453884-96f7-4897-95d2-acc10ee066bc', 'SP018', '8933619399091', 'Kẹo dẻo hương trái cây Hải Hà gói 100g', 'Kẹo dẻo hương trái cây Hải Hà gói 100g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 12000, 8500, 'https://picsum.photos/seed/SP018/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('c1bc8af1-3fb5-4999-aaae-bc22b2659857', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '09453884-96f7-4897-95d2-acc10ee066bc', 127, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('d75ddc32-f336-4047-869d-e50f35e5d3ac', 'SP019', '8939985435346', 'Sổ tay lò xo A6 Hải Tiến 120 trang', 'Sổ tay lò xo A6 Hải Tiến 120 trang chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 15000, 10500, 'https://picsum.photos/seed/SP019/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('94800c33-aec5-4756-8ca6-3ada28a73ae5', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'd75ddc32-f336-4047-869d-e50f35e5d3ac', 53, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('c954d826-c197-43d1-afec-112f5b99b130', 'SP020', '8934751079911', 'Tương ớt Chinsu chai 250g', 'Tương ớt Chinsu chai 250g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 15000, 11000, 'https://picsum.photos/seed/SP020/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('d4e3fb00-061a-4b82-960a-59da295b9a47', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'c954d826-c197-43d1-afec-112f5b99b130', 74, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('bbaee750-45d6-4e91-b706-435373012294', 'SP021', '8934251354278', 'Nước mắm Chinsu Nam Ngư 750ml', 'Nước mắm Chinsu Nam Ngư 750ml chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 45000, 34000, 'https://picsum.photos/seed/SP021/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('45cd32e6-c99e-4db0-9ef5-d4bbab5b5e05', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'bbaee750-45d6-4e91-b706-435373012294', 97, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('ae4a0da3-d3ca-4747-a44b-987cef3db63b', 'SP022', '8938084124118', 'Bánh quy gấu Meiji Hello Panda 50g', 'Bánh quy gấu Meiji Hello Panda 50g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 18000, 13000, 'https://picsum.photos/seed/SP022/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('3c641c41-1446-4fe0-9f8a-7cad75fd4110', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'ae4a0da3-d3ca-4747-a44b-987cef3db63b', 59, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('0ad39940-e070-41c1-b766-c44c02531b56', 'SP023', '8934935348740', 'Mì tôm Hảo Hảo Tôm Chua Cay gói 75g', 'Mì tôm Hảo Hảo Tôm Chua Cay gói 75g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 4500, 3300, 'https://picsum.photos/seed/SP023/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('9c318eed-3a01-4ce0-9fde-346f5f06c9d0', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '0ad39940-e070-41c1-b766-c44c02531b56', 43, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('0b1a839d-cf8a-47c1-8955-ed5da13be016', 'SP024', '8936400524278', 'Xà bông cục Coast cục 113g', 'Xà bông cục Coast cục 113g chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 18000, 13000, 'https://picsum.photos/seed/SP024/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('ca98c36e-34ea-4c5e-afb2-0de3d460b4cd', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '0b1a839d-cf8a-47c1-8955-ed5da13be016', 129, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('0d45fd99-6844-4a92-a643-6228c0994135', 'SP025', '8930112805982', 'Khăn lau đa năng microfiber lốc 3 cái', 'Khăn lau đa năng microfiber lốc 3 cái chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 25000, 18000, 'https://picsum.photos/seed/SP025/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('5388a9fe-cdca-48b6-aa3b-e46b2cc8d3dc', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '0d45fd99-6844-4a92-a643-6228c0994135', 130, 7, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('512b258f-d569-486f-9a96-4de3c9faf1cf', 'SP026', '8930450533158', 'Nước lau kính Sunlight chai 650ml', 'Nước lau kính Sunlight chai 650ml chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 25000, 18000, 'https://picsum.photos/seed/SP026/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('08070e35-fcae-484c-bbf3-8fb02e6680d7', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '512b258f-d569-486f-9a96-4de3c9faf1cf', 124, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('53adee94-7b76-4d93-8c8a-6c6dda6c5cbf', 'SP027', '8932322602563', 'Cây lau nhà tự vắt thông minh', 'Cây lau nhà tự vắt thông minh chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 125000, 92000, 'https://picsum.photos/seed/SP027/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('ecfedd52-ba6c-41d2-9770-8fd7bb576c38', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '53adee94-7b76-4d93-8c8a-6c6dda6c5cbf', 88, 7, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('e94b9c55-4f00-4337-a93f-5eab83c43a77', 'SP028', '8931607337543', 'Bánh mì tươi chà bông Staff 55g', 'Bánh mì tươi chà bông Staff 55g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 10000, 7500, 'https://picsum.photos/seed/SP028/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('0ffdf288-f577-4805-b643-57a3acc2b33a', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'e94b9c55-4f00-4337-a93f-5eab83c43a77', 77, 5, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('a9ea947b-d1e7-42c1-92a6-e2d89d8169ef', 'SP029', '8933654145868', 'Pin tiểu AA Panasonic lốc 4 viên', 'Pin tiểu AA Panasonic lốc 4 viên chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 16000, 11000, 'https://picsum.photos/seed/SP029/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('bbe49deb-3178-4124-a84b-a43ec3031fa2', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'a9ea947b-d1e7-42c1-92a6-e2d89d8169ef', 104, 5, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('ef2903e3-2bdd-4d08-bdb4-23428f2df37b', 'SP030', '8931429401965', 'Mì ăn liền Kokomi đại 90g', 'Mì ăn liền Kokomi đại 90g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 4000, 2900, 'https://picsum.photos/seed/SP030/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('74613705-4411-48d0-b06d-8c7d04d94fc3', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'ef2903e3-2bdd-4d08-bdb4-23428f2df37b', 100, 11, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('afb9add4-4981-4063-84c4-74aeeba0be97', 'SP031', '8939816934060', 'Sữa tươi TH True Milk ít đường 180ml', 'Sữa tươi TH True Milk ít đường 180ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 9000, 6800, 'https://picsum.photos/seed/SP031/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('212a72fb-7a32-436e-8f18-f38fd508d543', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'afb9add4-4981-4063-84c4-74aeeba0be97', 70, 10, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('f25b4490-4c5b-43c5-aad2-bb9bf270dac5', 'SP032', '8936159514846', 'Bột giặt Omo Red túi 4.1kg', 'Bột giặt Omo Red túi 4.1kg chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 215000, 175000, 'https://picsum.photos/seed/SP032/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('b5460aa8-5417-4dd9-97f7-f00ff6e3e989', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'f25b4490-4c5b-43c5-aad2-bb9bf270dac5', 103, 11, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('844fd6f9-d78b-4cc5-9321-38304396e5bd', 'SP033', '8934823662994', 'Xà bông cục Lifebuoy Đỏ 90g', 'Xà bông cục Lifebuoy Đỏ 90g chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 14000, 10500, 'https://picsum.photos/seed/SP033/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('fa638851-03f0-4c3d-ad9f-6b597dd1fe44', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '844fd6f9-d78b-4cc5-9321-38304396e5bd', 123, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('8c8e070e-c72e-45dd-8bf9-349d3bff8363', 'SP034', '8930443699577', 'Bông tẩy trang Silcot hộp 82 miếng', 'Bông tẩy trang Silcot hộp 82 miếng chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 35000, 26000, 'https://picsum.photos/seed/SP034/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('b3876d80-0404-427b-bf8b-71b95b6c5321', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '8c8e070e-c72e-45dd-8bf9-349d3bff8363', 133, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('3d5f1322-20b8-4d4e-96fb-6d2808ac7cf6', 'SP035', '8933872148951', 'Kéo học sinh SDI 125mm', 'Kéo học sinh SDI 125mm chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 12000, 8500, 'https://picsum.photos/seed/SP035/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('fe8b24cd-6786-4fb7-ba42-50979d8c6a18', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '3d5f1322-20b8-4d4e-96fb-6d2808ac7cf6', 80, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('3cabc159-7ce2-4246-b1a5-666e0fcfd8be', 'SP036', '8934332003791', 'Nước dùng lẩu Thái Haidilao gói 220g', 'Nước dùng lẩu Thái Haidilao gói 220g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 55000, 42000, 'https://picsum.photos/seed/SP036/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('e42631e2-4bc7-4130-803a-0a3272307f26', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '3cabc159-7ce2-4246-b1a5-666e0fcfd8be', 136, 11, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('eafe451c-4766-4766-998b-4a6c5745eecc', 'SP037', '8939367632016', 'Bút xóa kéo Thiên Long CT-04', 'Bút xóa kéo Thiên Long CT-04 chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 15000, 10500, 'https://picsum.photos/seed/SP037/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('0056504a-d101-45a9-aba9-7f66b6f7078f', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'eafe451c-4766-4766-998b-4a6c5745eecc', 76, 7, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('2a4665fc-2cf0-42fe-ada3-0e26e33bb27b', 'SP038', '8938708317278', 'Khăn giấy rút Bless You 2 lớp gói 250 tờ', 'Khăn giấy rút Bless You 2 lớp gói 250 tờ chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 29000, 21000, 'https://picsum.photos/seed/SP038/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('f0a9a024-97a5-4f2c-aed9-881b97f91bb2', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '2a4665fc-2cf0-42fe-ada3-0e26e33bb27b', 101, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('d4a3683f-07ad-4d8c-a009-a03a17ef7a79', 'SP039', '8939868727743', 'Bình đựng nước thủy tinh LocknLock 1.2L', 'Bình đựng nước thủy tinh LocknLock 1.2L chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 85000, 62000, 'https://picsum.photos/seed/SP039/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('a805179b-3d84-4fbb-868f-700131516857', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'd4a3683f-07ad-4d8c-a009-a03a17ef7a79', 90, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('dabffc41-1c45-4508-8d45-eb91cfb1812a', 'SP040', '8937347143455', 'Sữa đậu nành Fami Canxi 200ml', 'Sữa đậu nành Fami Canxi 200ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 7000, 5000, 'https://picsum.photos/seed/SP040/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('317bc265-92ca-4682-81c4-db62b8b91fa5', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'dabffc41-1c45-4508-8d45-eb91cfb1812a', 40, 7, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('1e3ed497-c953-4ad1-8b83-8a4813bfb311', 'SP041', '8932362316658', 'Nước tương Chinsu tỏi ớt 250ml', 'Nước tương Chinsu tỏi ớt 250ml chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 18000, 13500, 'https://picsum.photos/seed/SP041/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('20b71c60-1556-491d-912d-d58fec0fc709', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '1e3ed497-c953-4ad1-8b83-8a4813bfb311', 139, 11, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('2218efd4-c15f-4254-b914-ca10f244f30a', 'SP042', '8930366909670', 'Hạt điều rang muối O&T 150g', 'Hạt điều rang muối O&T 150g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 65000, 48000, 'https://picsum.photos/seed/SP042/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('b240530d-4874-4617-a3e8-e9d1ea5044d7', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '2218efd4-c15f-4254-b914-ca10f244f30a', 110, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('2518f4e4-2581-4fba-8420-22689d642a1c', 'SP043', '8936688937346', 'Gạo thơm lài sữa dẻo túi 5kg', 'Gạo thơm lài sữa dẻo túi 5kg chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 110000, 85000, 'https://picsum.photos/seed/SP043/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('8bbad6ad-405a-4931-959a-e7a8e49f8e38', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '2518f4e4-2581-4fba-8420-22689d642a1c', 144, 5, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('42b3034b-59b9-45ea-bc66-887e46ab1b5a', 'SP044', '8936562729806', 'Mì trộn cay Samyang Hàn Quốc 140g', 'Mì trộn cay Samyang Hàn Quốc 140g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 28000, 20500, 'https://picsum.photos/seed/SP044/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('ab92d259-1599-4d46-b7ce-495b7d586e3b', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '42b3034b-59b9-45ea-bc66-887e46ab1b5a', 26, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('f06dbc13-b8fb-43a3-88a9-63d97ecaa8dc', 'SP045', '8936272046537', 'Khoai tây chiên Lay''s Vị Tự Nhiên 95g', 'Khoai tây chiên Lay''s Vị Tự Nhiên 95g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 20000, 15000, 'https://picsum.photos/seed/SP045/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('4d4d7b1c-3702-4503-a1cf-771d5a0d68fa', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'f06dbc13-b8fb-43a3-88a9-63d97ecaa8dc', 103, 10, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('0d4d1bec-0af2-436c-a2eb-d860c40aa5d9', 'SP046', '8936464170805', 'Băng vệ sinh Diana siêu thấm cánh 8 miếng', 'Băng vệ sinh Diana siêu thấm cánh 8 miếng chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 18000, 13000, 'https://picsum.photos/seed/SP046/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('998a7583-8bdb-48e0-8a8e-4c74dc889d37', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '0d4d1bec-0af2-436c-a2eb-d860c40aa5d9', 77, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('e3b88584-a3b0-4644-b2dc-43803acfde8b', 'SP047', '8931003309232', 'Nước súc miệng Listerine Cool Mint 250ml', 'Nước súc miệng Listerine Cool Mint 250ml chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 58000, 44000, 'https://picsum.photos/seed/SP047/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('00f051c8-0acb-478c-ba48-4d9b10bc1997', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'e3b88584-a3b0-4644-b2dc-43803acfde8b', 141, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('45003945-984a-41bd-8a72-7c63990e2abe', 'SP048', '8931937452991', 'Lăn khử mùi Nivea nam khô thoáng 50ml', 'Lăn khử mùi Nivea nam khô thoáng 50ml chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 52000, 39000, 'https://picsum.photos/seed/SP048/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('9bb2c236-65a9-4fae-88c5-8ab53bb76207', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '45003945-984a-41bd-8a72-7c63990e2abe', 61, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('abccc7af-6f45-4a1a-8cf7-ea1489cf88c4', 'SP049', '8931904966319', 'Bia Tiger lon 330ml', 'Bia Tiger lon 330ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 18000, 14000, 'https://picsum.photos/seed/SP049/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('4ba388c7-1cd1-4057-b529-bb2a188a59db', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'abccc7af-6f45-4a1a-8cf7-ea1489cf88c4', 82, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('d5a03acc-5682-4da5-96d4-8d9ae5f9694f', 'SP050', '8934919058651', 'Hạt nêm Knorr thịt thăn ống xương 400g', 'Hạt nêm Knorr thịt thăn ống xương 400g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 38000, 28500, 'https://picsum.photos/seed/SP050/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('09528217-a2c2-47a9-a502-43cc78ef92df', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'd5a03acc-5682-4da5-96d4-8d9ae5f9694f', 149, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('df28c66b-aabf-4a6c-8b1f-735c96fa306b', 'SP051', '8935067165726', 'Hộp thực phẩm chữ nhật Duy Tân 500ml', 'Hộp thực phẩm chữ nhật Duy Tân 500ml chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 15000, 10000, 'https://picsum.photos/seed/SP051/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('bb2d6e2d-a5b0-401d-a36d-3a74c07e71f2', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'df28c66b-aabf-4a6c-8b1f-735c96fa306b', 65, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('ebc43d92-0748-4257-87ac-0b3229a48021', 'SP052', '8934987769453', 'Nước rửa chén Sunlight Trà Xanh 750g', 'Nước rửa chén Sunlight Trà Xanh 750g chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 28000, 20000, 'https://picsum.photos/seed/SP052/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('23245b3a-c573-492d-b603-ae9fb9c9ff2b', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'ebc43d92-0748-4257-87ac-0b3229a48021', 42, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('81896438-c839-46b3-ab86-7af01caf2233', 'SP053', '8937379965075', 'Nước Cam ép Twister 450ml', 'Nước Cam ép Twister 450ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 12000, 8500, 'https://picsum.photos/seed/SP053/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('1e9988c5-7b45-4b49-883b-635b46de7fe1', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '81896438-c839-46b3-ab86-7af01caf2233', 66, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('0b3f13f9-63af-4cdd-b918-ed90f78b44fc', 'SP054', '8933545494808', 'Nước tẩy bồn cầu Vim diệt khuẩn 900ml', 'Nước tẩy bồn cầu Vim diệt khuẩn 900ml chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 36000, 27000, 'https://picsum.photos/seed/SP054/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('075c62be-3c9e-41ea-b795-e32b6285a753', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '0b3f13f9-63af-4cdd-b918-ed90f78b44fc', 68, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('603b4521-469f-4fbc-a991-037e0802875b', 'SP055', '8933678377701', 'Bánh Choco-Pie Orion hộp 6 cái', 'Bánh Choco-Pie Orion hộp 6 cái chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 38000, 29000, 'https://picsum.photos/seed/SP055/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('d31388e3-217f-415c-9970-c1d33bbc3d66', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '603b4521-469f-4fbc-a991-037e0802875b', 95, 8, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('096224e2-8385-4961-b6ac-067ffd602aee', 'SP056', '8936349578856', 'Muối sấy Tây Ninh DH Foods hũ 120g', 'Muối sấy Tây Ninh DH Foods hũ 120g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 20000, 14000, 'https://picsum.photos/seed/SP056/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('0ed925ed-421c-4f1e-bbab-5a088b6d31f5', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '096224e2-8385-4961-b6ac-067ffd602aee', 104, 10, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('d7fa08cc-63b2-481c-a328-6cfc8628e4d9', 'SP057', '8937444313518', 'Bàn chải đánh răng P/S lông tơ mềm mại', 'Bàn chải đánh răng P/S lông tơ mềm mại chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 15000, 10000, 'https://picsum.photos/seed/SP057/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('6c138216-2d81-45ec-b7ea-7729e04a1412', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'd7fa08cc-63b2-481c-a328-6cfc8628e4d9', 67, 8, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('ef7e145f-c0ba-44d4-8701-b59a17eecb73', 'SP058', '8933749894134', 'Bút bi Thiên Long TL-027 xanh', 'Bút bi Thiên Long TL-027 xanh chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 4000, 2500, 'https://picsum.photos/seed/SP058/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('ec9c89d2-bbfd-4d9f-8c38-0a5b5dcafbdf', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'ef7e145f-c0ba-44d4-8701-b59a17eecb73', 78, 10, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('d1677a81-90c3-4a80-9964-80c99004aebe', 'SP059', '8932408240084', 'Kẹo cao su Doublemint hũ 58g', 'Kẹo cao su Doublemint hũ 58g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 25000, 18500, 'https://picsum.photos/seed/SP059/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('82e06961-a6bc-425d-a09d-1de117d5e9db', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'd1677a81-90c3-4a80-9964-80c99004aebe', 52, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('8520b7f3-b7e2-470e-bb6a-5f2aef529212', 'SP060', '8937109477752', 'Sữa rửa mặt Acnes ngừa mụn 100g', 'Sữa rửa mặt Acnes ngừa mụn 100g chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 65000, 48000, 'https://picsum.photos/seed/SP060/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('bbf1c17a-4bc9-42b1-9b26-0e8523031596', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '8520b7f3-b7e2-470e-bb6a-5f2aef529212', 33, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('4c5afb7d-27e1-40e4-ae57-1238730ee587', 'SP061', '8937116719022', 'Bánh xốp Nabati phô mai 140g', 'Bánh xốp Nabati phô mai 140g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 15000, 11000, 'https://picsum.photos/seed/SP061/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('8a7d3db2-124e-4fb4-bf1d-63e341cabfc8', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '4c5afb7d-27e1-40e4-ae57-1238730ee587', 97, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('b62dcf80-352b-43e7-926c-a58905c30915', 'SP062', '8933186999386', 'Nước tinh khiết Aquafina 500ml', 'Nước tinh khiết Aquafina 500ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 6000, 4000, 'https://picsum.photos/seed/SP062/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('855546e8-75ba-4b3e-ab92-e2a7ebcbc93f', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'b62dcf80-352b-43e7-926c-a58905c30915', 135, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('c567a034-7796-4a07-99b2-192966d4f0b2', 'SP063', '8934964990913', 'Bia Heineken lon 330ml', 'Bia Heineken lon 330ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 22000, 17500, 'https://picsum.photos/seed/SP063/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('443f88f4-a290-42b4-81af-a168659d6433', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'c567a034-7796-4a07-99b2-192966d4f0b2', 74, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('a5ca5028-fd55-464e-9c6a-39f218bd24bb', 'SP064', '8931232812067', 'Vở kẻ ngang Hồng Hà Class 80 trang', 'Vở kẻ ngang Hồng Hà Class 80 trang chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 8000, 5500, 'https://picsum.photos/seed/SP064/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('785f37da-1f69-4ba4-882d-11d0369dd7d2', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'a5ca5028-fd55-464e-9c6a-39f218bd24bb', 140, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('8c353236-835a-4b9e-b8df-7824a0de3b3c', 'SP065', '8930344713493', 'Chổi nhựa quét nhà kèm hốt rác', 'Chổi nhựa quét nhà kèm hốt rác chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 48000, 35000, 'https://picsum.photos/seed/SP065/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('6b9fca60-07a3-4dfb-a98e-c1d846a4b2c9', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '8c353236-835a-4b9e-b8df-7824a0de3b3c', 128, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('1ad8c7e9-3aec-48c3-aaf2-a1c37fe84e91', 'SP066', '8938324210249', 'Bim bim Oishi tôm cay 80g', 'Bim bim Oishi tôm cay 80g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 8000, 5500, 'https://picsum.photos/seed/SP066/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('a4056bfe-cf3b-475f-bcf1-1756c5f78087', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '1ad8c7e9-3aec-48c3-aaf2-a1c37fe84e91', 93, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('de987731-c97d-4a31-95be-8290b6dfac30', 'SP067', '8931746488771', 'Bàn chải chà sàn cán gỗ tròn', 'Bàn chải chà sàn cán gỗ tròn chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 16000, 11000, 'https://picsum.photos/seed/SP067/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('e81676a1-3a8a-4159-9b0d-89c0b5e60e35', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'de987731-c97d-4a31-95be-8290b6dfac30', 30, 11, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('a94993a3-c5ad-4803-8f11-fc7a78452731', 'SP068', '8935940139904', 'Bột bắp tài ký gói 150g', 'Bột bắp tài ký gói 150g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 8000, 5500, 'https://picsum.photos/seed/SP068/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('65b9c476-f948-4f9c-82cb-1c0813081810', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'a94993a3-c5ad-4803-8f11-fc7a78452731', 30, 7, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('1deac558-d548-4005-a126-6536f406fbfb', 'SP069', '8937874296717', 'Sữa chua Vinamilk có đường hũ 100g', 'Sữa chua Vinamilk có đường hũ 100g chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 7000, 5200, 'https://picsum.photos/seed/SP069/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('244fb0d5-0a4a-42b8-97d9-7f1abbefd57e', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '1deac558-d548-4005-a126-6536f406fbfb', 109, 11, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('185f7b1c-fdff-43c8-94b2-1cf6f99ede3c', 'SP070', '8935512567468', 'Khô gà xé lá chanh túi 100g', 'Khô gà xé lá chanh túi 100g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 35000, 24000, 'https://picsum.photos/seed/SP070/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('d20454df-4aa4-4380-83ef-44e8cf1c52ec', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '185f7b1c-fdff-43c8-94b2-1cf6f99ede3c', 29, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('fc03b20a-ebe6-48f9-a6c9-2410537552fa', 'SP071', '8931545168087', 'Coca Cola 320ml', 'Coca Cola 320ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 10000, 7000, 'https://picsum.photos/seed/SP071/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('b1a6a36c-02e4-4843-95ed-a72ef32aa7b5', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'fc03b20a-ebe6-48f9-a6c9-2410537552fa', 125, 5, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('0841a306-eab4-4b8c-9329-9a0287dead47', 'SP072', '8933859770348', 'Nước lau kiếng GIFT trà xanh 580ml', 'Nước lau kiếng GIFT trà xanh 580ml chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 22000, 16000, 'https://picsum.photos/seed/SP072/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('a7743a58-a627-49e1-9846-c70c14c78ac4', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '0841a306-eab4-4b8c-9329-9a0287dead47', 53, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('42d1c085-7097-4afd-9d11-0a44300d5213', 'SP073', '8937710932480', 'Kem đánh răng Colgate Ngừa Sâu Răng 225g', 'Kem đánh răng Colgate Ngừa Sâu Răng 225g chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 38000, 28000, 'https://picsum.photos/seed/SP073/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('15c81db4-9392-4ac6-b45a-8c4786c11d20', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '42d1c085-7097-4afd-9d11-0a44300d5213', 124, 6, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('f09c6afe-ca9f-4e73-9c3d-9645edfae691', 'SP074', '8933171274846', 'Sốt Mayonnaise Kewpie chai 130g', 'Sốt Mayonnaise Kewpie chai 130g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 28000, 21000, 'https://picsum.photos/seed/SP074/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('78fd6949-7009-4e42-80a1-db475cc00fc2', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'f09c6afe-ca9f-4e73-9c3d-9645edfae691', 143, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('3bad0664-ccdf-4f2d-ba11-8359e852c13b', 'SP075', '8933782639821', 'Giấy vệ sinh Pulppy cuộn không lõi lốc 10', 'Giấy vệ sinh Pulppy cuộn không lõi lốc 10 chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 72000, 54000, 'https://picsum.photos/seed/SP075/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('7847191c-a580-4c22-bc85-e9a57b2463ac', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '3bad0664-ccdf-4f2d-ba11-8359e852c13b', 90, 11, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('113980fe-4189-4586-a047-96f5d10ca799', 'SP076', '8935840449972', 'Trà Oolong Tea+ Plus 455ml', 'Trà Oolong Tea+ Plus 455ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 10000, 7200, 'https://picsum.photos/seed/SP076/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('465694c9-d1b4-46a8-bce8-b90c0666fd83', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '113980fe-4189-4586-a047-96f5d10ca799', 134, 13, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('c6d1510b-5e85-4a1b-8e09-e7a0682965b6', 'SP077', '8937558867533', 'Sô cô la Snickers thanh 51g', 'Sô cô la Snickers thanh 51g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 16000, 12000, 'https://picsum.photos/seed/SP077/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('45f68626-70b1-4601-b597-cd2cb49e432b', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'c6d1510b-5e85-4a1b-8e09-e7a0682965b6', 118, 8, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('bab92644-68be-49ca-92b1-fdf217df0389', 'SP078', '8936057662702', 'Hồ dán nước Thiên Long chai nhỏ', 'Hồ dán nước Thiên Long chai nhỏ chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 5000, 3200, 'https://picsum.photos/seed/SP078/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('92032e11-7482-48e4-ae5f-484ff1075314', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'bab92644-68be-49ca-92b1-fdf217df0389', 148, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('4549baf6-0c58-48a0-8412-02058da65282', 'SP079', '8935171870262', 'Hộp nhựa đựng thực phẩm tròn Duy Tân 1L', 'Hộp nhựa đựng thực phẩm tròn Duy Tân 1L chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 22000, 15000, 'https://picsum.photos/seed/SP079/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('bdaf0da6-fade-4072-b4de-9ab0f732d78e', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '4549baf6-0c58-48a0-8412-02058da65282', 39, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('8519833a-1428-4dbd-a2a7-69379d63bd17', 'SP080', '8934596158657', 'Móc phơi quần áo inox lốc 10 cái', 'Móc phơi quần áo inox lốc 10 cái chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 45000, 32000, 'https://picsum.photos/seed/SP080/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('9eea90c5-f878-48f0-8456-14d0dd16be69', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '8519833a-1428-4dbd-a2a7-69379d63bd17', 29, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('fe092e84-3474-40e2-9231-d0b0684cc47f', 'SP081', '8931343161172', 'Nước tẩy trang L''Oreal Micellar Water 400ml', 'Nước tẩy trang L''Oreal Micellar Water 400ml chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 185000, 145000, 'https://picsum.photos/seed/SP081/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('1e381d8e-fe19-454d-958a-1ba8fd1eeb97', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'fe092e84-3474-40e2-9231-d0b0684cc47f', 96, 5, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('ef36d8d6-caa6-4237-a885-038ac639130a', 'SP082', '8930504556238', 'Snack rong biển Tao Kae Noi 3.2g', 'Snack rong biển Tao Kae Noi 3.2g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 10000, 7000, 'https://picsum.photos/seed/SP082/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('48663399-c65b-499e-82f4-704538d62757', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'ef36d8d6-caa6-4237-a885-038ac639130a', 125, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('fbd4fa79-a651-40a5-a9b2-1f3c560da15d', 'SP083', '8932221969379', 'Kẹo sâm không đường Korea 150g', 'Kẹo sâm không đường Korea 150g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 30000, 21000, 'https://picsum.photos/seed/SP083/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('e4f6ca47-136b-44e4-a7e8-1fb9f539f44c', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'fbd4fa79-a651-40a5-a9b2-1f3c560da15d', 56, 8, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('3dcd2a4d-6d9e-444f-b3e2-13aae641f3c1', 'SP084', '8937474074821', 'Bật lửa gas Clipper chính hãng', 'Bật lửa gas Clipper chính hãng chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 12000, 8000, 'https://picsum.photos/seed/SP084/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('fa6f5e0d-abb7-4370-8b41-533c3fadb4ec', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '3dcd2a4d-6d9e-444f-b3e2-13aae641f3c1', 133, 10, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('a67e3ced-4c5c-4da8-8f5a-f3ea62989984', 'SP085', '8939464743671', 'Bông ngoáy tai Niva hộp 150 que', 'Bông ngoáy tai Niva hộp 150 que chất lượng cao, thuộc nhóm ngành hàng Chăm sóc cá nhân.', 14000, 9500, 'https://picsum.photos/seed/SP085/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('0dea8642-4f61-4abc-bb04-3cab2defb8e7', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'a67e3ced-4c5c-4da8-8f5a-f3ea62989984', 80, 11, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('a277cf22-86d5-4fc0-b3d0-2b3f8bf5f380', 'SP086', '8939594406409', 'Nước tăng lực Sting Dâu 320ml', 'Nước tăng lực Sting Dâu 320ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 11000, 8000, 'https://picsum.photos/seed/SP086/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('92b293af-340d-4290-80a6-f2d913410ef0', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'a277cf22-86d5-4fc0-b3d0-2b3f8bf5f380', 32, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('aeeba31a-13a0-4f02-b2ff-85eb32a6d769', 'SP087', '8937439533942', 'Túi rác tự hủy sinh học Opec lốc 3 cuộn', 'Túi rác tự hủy sinh học Opec lốc 3 cuộn chất lượng cao, thuộc nhóm ngành hàng Hóa phẩm & Vệ sinh.', 35000, 25000, 'https://picsum.photos/seed/SP087/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('83c3ce30-feba-4957-951b-0d29837737f2', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'aeeba31a-13a0-4f02-b2ff-85eb32a6d769', 44, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('1731973b-a253-4320-b316-cd8a26826c65', 'SP088', '8930470952145', 'Băng keo trong Hải Âu cuộn 1.8cm', 'Băng keo trong Hải Âu cuộn 1.8cm chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 6000, 4200, 'https://picsum.photos/seed/SP088/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('addba867-4b02-442d-8b28-a744691ce185', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '1731973b-a253-4320-b316-cd8a26826c65', 126, 7, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('559115da-d248-43a6-a0e6-7a5630c4954a', 'SP089', '8933285884247', 'Nước khoáng có ga Vĩnh Hảo 500ml', 'Nước khoáng có ga Vĩnh Hảo 500ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 8000, 5500, 'https://picsum.photos/seed/SP089/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('19defa68-bb61-4a5a-94ba-ffa4c21db33e', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '559115da-d248-43a6-a0e6-7a5630c4954a', 95, 10, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('0769523e-5136-4e86-aecf-f2197a781c6e', 'SP090', '8931712368516', 'Bút dạ quang Thiên Long HL-03 vàng', 'Bút dạ quang Thiên Long HL-03 vàng chất lượng cao, thuộc nhóm ngành hàng Văn phòng phẩm.', 12000, 8500, 'https://picsum.photos/seed/SP090/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('60ad9fd0-b71f-4742-baff-586d77e1f390', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '0769523e-5136-4e86-aecf-f2197a781c6e', 23, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('429a08df-9243-4119-8ab1-3599b762ba7a', 'SP091', '8938175496513', 'Ổ cắm điện Lioa 4 ổ cắm 3m dây', 'Ổ cắm điện Lioa 4 ổ cắm 3m dây chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 95000, 72000, 'https://picsum.photos/seed/SP091/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('fa5442ab-35c3-4c8e-aae5-87cda3038cec', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '429a08df-9243-4119-8ab1-3599b762ba7a', 140, 5, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('fd6d661c-1417-4abd-9e40-3c76bdf77880', 'SP092', '8939859317461', 'Sữa chua uống Proby Vinamilk 65ml', 'Sữa chua uống Proby Vinamilk 65ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 5000, 3800, 'https://picsum.photos/seed/SP092/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('71c55150-9847-4c09-97eb-627fb62ff43b', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'fd6d661c-1417-4abd-9e40-3c76bdf77880', 55, 5, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('4892d3ec-5e17-4033-a458-183a0c2cf3b9', 'SP093', '8930471138267', 'Kẹo dẻo Haribo Goldbears 80g', 'Kẹo dẻo Haribo Goldbears 80g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 22000, 16000, 'https://picsum.photos/seed/SP093/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('d0589618-fe03-4adb-87bd-6a5c56282e61', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '4892d3ec-5e17-4033-a458-183a0c2cf3b9', 114, 15, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('8784f6ce-9942-46b6-9931-a331093624e9', 'SP094', '8938692617964', 'Bông lan cuộn Solite dâu hộp 360g', 'Bông lan cuộn Solite dâu hộp 360g chất lượng cao, thuộc nhóm ngành hàng Bánh kẹo & Ăn vặt.', 42000, 32000, 'https://picsum.photos/seed/SP094/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('4853ae98-5db2-44e7-ad34-9cc759d0a9e3', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '8784f6ce-9942-46b6-9931-a331093624e9', 28, 10, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('1c388304-3075-4a1a-969a-4c744253167d', 'SP095', '8933773515850', 'Dầu ăn Tường An Cooking Oil 1L', 'Dầu ăn Tường An Cooking Oil 1L chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 48000, 37000, 'https://picsum.photos/seed/SP095/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('b8b6602d-a5df-4361-87b5-750a84b7debe', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '1c388304-3075-4a1a-969a-4c744253167d', 121, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('f974219d-3940-437a-9954-6620b0872124', 'SP096', '8933171390053', 'Bột ngọt Ajinomoto gói 454g', 'Bột ngọt Ajinomoto gói 454g chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 32000, 24000, 'https://picsum.photos/seed/SP096/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('72a0c04f-de2e-4305-a8c7-280076c6b7e5', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'f974219d-3940-437a-9954-6620b0872124', 52, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('24e8af84-5b6a-47af-9222-0f40394872be', 'SP097', '8933183933529', 'Nước tương Tam Thái Tử Nhị Ca 500ml', 'Nước tương Tam Thái Tử Nhị Ca 500ml chất lượng cao, thuộc nhóm ngành hàng Gia vị & Đồ khô.', 16000, 12000, 'https://picsum.photos/seed/SP097/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('beaf2112-fcc6-4b48-8880-7a7d1b1114ba', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '24e8af84-5b6a-47af-9222-0f40394872be', 20, 9, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('b95e888d-a4bf-4caa-8770-27d03fe888fe', 'SP098', '8932284210205', 'Trà xanh Không Độ 500ml', 'Trà xanh Không Độ 500ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 10000, 7200, 'https://picsum.photos/seed/SP098/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('c821afed-09ce-43b8-bf75-161b4a45b22f', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'b95e888d-a4bf-4caa-8770-27d03fe888fe', 80, 14, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('33e84687-2499-43dc-9c98-aa00d5cbc3a5', 'SP099', '8935024026811', 'Cà phê sữa đá Highland lon 235ml', 'Cà phê sữa đá Highland lon 235ml chất lượng cao, thuộc nhóm ngành hàng Đồ uống.', 19000, 14500, 'https://picsum.photos/seed/SP099/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('6120098b-8c3e-495d-a528-1225aca4bcfc', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '33e84687-2499-43dc-9c98-aa00d5cbc3a5', 141, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

INSERT INTO "Products" ("Id", "Code", "Barcode", "Name", "Description", "SellingPrice", "ImportPrice", "ImageUrl", "IsActive", "CreatedAt", "CreatedBy")
VALUES ('42c748f8-6cd5-445c-95a2-5329e1f0a30f', 'SP100', '8935891783908', 'Găng tay cao su rửa chén siêu dai', 'Găng tay cao su rửa chén siêu dai chất lượng cao, thuộc nhóm ngành hàng Gia dụng & Tiện ích.', 15000, 10000, 'https://picsum.photos/seed/SP100/200', true, NOW(), 'System')
ON CONFLICT ("Code") DO UPDATE SET "Name" = EXCLUDED."Name", "SellingPrice" = EXCLUDED."SellingPrice";

INSERT INTO "BranchInventories" ("Id", "BranchId", "ProductId", "Quantity", "MinimumStockLevel", "CreatedAt", "CreatedBy")
VALUES ('5581edce-49dd-40f1-8093-708ad7ca3840', 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', '42c748f8-6cd5-445c-95a2-5329e1f0a30f', 97, 12, NOW(), 'System')
ON CONFLICT ("BranchId", "ProductId") DO NOTHING;

COMMIT;
