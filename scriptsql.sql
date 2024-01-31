-- hàm tạo giá trị cho cột document FTS
create or replace function nhanVien_trigger_document_funct()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
declare
	tenChucVu varchar := (select "TenChucVu" from "ChucVu" where "Id" = NEW."ChucVuId");
	tenPhongBan varchar := (select "TenPhongBan" from "PhongBan" where "Id" = NEW."PhongBanId");
begin 
	NEW."Document" = setweight(to_tsvector('simple',unaccent(LOWER(coalesce(NEW."HoVaTen",'')))),'A') ||
					 setweight(to_tsvector('simple',unaccent(LOWER(coalesce(tenChucVu)))),'C') ||
					 setweight(to_tsvector('simple',unaccent(LOWER(coalesce(tenPhongBan)))),'C');

	return NEW;
end $$;

-- trigger tự động tạo giá trị document
CREATE TRIGGER nhanVien_trigger_document before insert or update
of "HoVaTen", "PhongBanId", "ChucVuId" on "NhanVien" for each row
execute procedure nhanVien_trigger_document_funct()

-- query cập nhật cột Document									  
update "NhanVien"
set "Document" = (select setweight(to_tsvector('simple',unaccent(LOWER(coalesce(n1."HoVaTen",'')))),'A') ||
						 setweight(to_tsvector('simple',unaccent(LOWER(coalesce(c."TenChucVu")))),'C') ||
				         setweight(to_tsvector('simple',unaccent(LOWER(coalesce(p."TenPhongBan")))),'C')
				  	from "NhanVien" as n1
					join "ChucVu" as c on c."Id" = n1."ChucVuId"
					join "PhongBan" as p on p."Id" = n1."PhongBanId"
					where n1."Id" = "NhanVien"."Id")
									  
-- so sánh điểm của hàm setweight giá trị  A B C D 
SELECT
ts_rank(setweight(to_tsvector('english', 'The quick brown fox'), 'A'), to_tsquery('english', 'quick')) AS a,
ts_rank(setweight(to_tsvector('english', 'The quick brown fox'), 'B'), to_tsquery('english', 'quick')) AS b,    
ts_rank(setweight(to_tsvector('english', 'The quick brown fox'), 'C'), to_tsquery('english', 'quick')) AS c,
ts_rank(setweight(to_tsvector('english', 'The quick brown fox'), 'D'), to_tsquery('english', 'quick')) AS d

-- tạo index cho cột Document tsvector
CREATE INDEX FTS_NhanVien_Document ON "NhanVien" USING gin("Document");
