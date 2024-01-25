PGDMP  (                     |            QuanLyNhanVien    16.1    16.0     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    24576    QuanLyNhanVien    DATABASE     �   CREATE DATABASE "QuanLyNhanVien" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United States.1252';
     DROP DATABASE "QuanLyNhanVien";
                postgres    false            P           1247    24596    year    DOMAIN     k   CREATE DOMAIN public.year AS integer
	CONSTRAINT year_check CHECK (((VALUE >= 1901) AND (VALUE <= 2155)));
    DROP DOMAIN public.year;
       public          postgres    false            �            1255    24598    _group_concat(text, text)    FUNCTION     �   CREATE FUNCTION public._group_concat(text, text) RETURNS text
    LANGUAGE sql IMMUTABLE
    AS $_$
SELECT CASE
  WHEN $2 IS NULL THEN $1
  WHEN $1 IS NULL THEN $2
  ELSE $1 || ', ' || $2
END
$_$;
 0   DROP FUNCTION public._group_concat(text, text);
       public          postgres    false            �            1255    24605    last_updated()    FUNCTION     �   CREATE FUNCTION public.last_updated() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.last_update = CURRENT_TIMESTAMP;
    RETURN NEW;
END $$;
 %   DROP FUNCTION public.last_updated();
       public          postgres    false            X           1255    24615    group_concat(text) 	   AGGREGATE     c   CREATE AGGREGATE public.group_concat(text) (
    SFUNC = public._group_concat,
    STYPE = text
);
 *   DROP AGGREGATE public.group_concat(text);
       public          postgres    false    219            �            1259    24578    NhanVien    TABLE     �   CREATE TABLE public."NhanVien" (
    "Id" integer NOT NULL,
    "HoVaTen" character varying(50) NOT NULL,
    "NgaySinh" date NOT NULL,
    "DienThoai" character varying(11),
    "ChucVu" character varying(20),
    "PhongBanId" integer
);
    DROP TABLE public."NhanVien";
       public         heap    postgres    false            �            1259    24577    NhanVien_Id_seq    SEQUENCE     �   CREATE SEQUENCE public."NhanVien_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public."NhanVien_Id_seq";
       public          postgres    false    216            �           0    0    NhanVien_Id_seq    SEQUENCE OWNED BY     I   ALTER SEQUENCE public."NhanVien_Id_seq" OWNED BY public."NhanVien"."Id";
          public          postgres    false    215            �            1259    32787    PhongBan    TABLE     p   CREATE TABLE public."PhongBan" (
    "Id" integer NOT NULL,
    "TenPhongBan" character varying(20) NOT NULL
);
    DROP TABLE public."PhongBan";
       public         heap    postgres    false            �            1259    32786    PhongBan_Id_seq    SEQUENCE     �   CREATE SEQUENCE public."PhongBan_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public."PhongBan_Id_seq";
       public          postgres    false    218            �           0    0    PhongBan_Id_seq    SEQUENCE OWNED BY     I   ALTER SEQUENCE public."PhongBan_Id_seq" OWNED BY public."PhongBan"."Id";
          public          postgres    false    217            &           2604    24581    NhanVien Id    DEFAULT     p   ALTER TABLE ONLY public."NhanVien" ALTER COLUMN "Id" SET DEFAULT nextval('public."NhanVien_Id_seq"'::regclass);
 >   ALTER TABLE public."NhanVien" ALTER COLUMN "Id" DROP DEFAULT;
       public          postgres    false    216    215    216            '           2604    32790    PhongBan Id    DEFAULT     p   ALTER TABLE ONLY public."PhongBan" ALTER COLUMN "Id" SET DEFAULT nextval('public."PhongBan_Id_seq"'::regclass);
 >   ALTER TABLE public."PhongBan" ALTER COLUMN "Id" DROP DEFAULT;
       public          postgres    false    218    217    218            �          0    24578    NhanVien 
   TABLE DATA           f   COPY public."NhanVien" ("Id", "HoVaTen", "NgaySinh", "DienThoai", "ChucVu", "PhongBanId") FROM stdin;
    public          postgres    false    216   A       �          0    32787    PhongBan 
   TABLE DATA           9   COPY public."PhongBan" ("Id", "TenPhongBan") FROM stdin;
    public          postgres    false    218   �       �           0    0    NhanVien_Id_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('public."NhanVien_Id_seq"', 114, true);
          public          postgres    false    215            �           0    0    PhongBan_Id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public."PhongBan_Id_seq"', 3, true);
          public          postgres    false    217            )           2606    24583    NhanVien NhanVien_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public."NhanVien"
    ADD CONSTRAINT "NhanVien_pkey" PRIMARY KEY ("Id");
 D   ALTER TABLE ONLY public."NhanVien" DROP CONSTRAINT "NhanVien_pkey";
       public            postgres    false    216            +           2606    32792    PhongBan PhongBan_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public."PhongBan"
    ADD CONSTRAINT "PhongBan_pkey" PRIMARY KEY ("Id");
 D   ALTER TABLE ONLY public."PhongBan" DROP CONSTRAINT "PhongBan_pkey";
       public            postgres    false    218            ,           2606    32793    NhanVien fk_nhanvien_phongban    FK CONSTRAINT     �   ALTER TABLE ONLY public."NhanVien"
    ADD CONSTRAINT fk_nhanvien_phongban FOREIGN KEY ("PhongBanId") REFERENCES public."PhongBan"("Id");
 I   ALTER TABLE ONLY public."NhanVien" DROP CONSTRAINT fk_nhanvien_phongban;
       public          postgres    false    216    218    4651            �   W  x��U�n�@]�|��@�<�{�K�D�Tu�MZ;u�%NR�F!6�*�
рjW�X��K�����^Ԛ$���;��s�ؕ�O�Ø�c6�qD�s������z2�_b�,�C��qZ����8b�����y�O���5!�h�o��y����S�l�(I�tn:�G/��=����G
Q5��i�c�<}��Q������F$f0���n)}ZL�?x^��IN��ѥ~�}�E���6���ge᮪>�6�>����]�+�7���(�7"AW'y���d�#7 �P$���(���(_���n6�C�����Q��N[v
��μ:�z��ٿ�NZ���p��a���pݴ2���i��R.+�9�HQtu�/�#��3(��&\HGK�6d�H���N���tA�;��"벊��!�Z�rTx��ḋ<�HI�&:7�;.%Îv����g�}�ǒP(�4��_��O����6�d\#��^[���*��eA _A�~�5A�SgM�9kPz9)m��񵡭<}o	��&��.�ly��E�!��f��$L&��ʨ
=���7H�$_�B]T��j�XbJу�	��(�����i�6oJ�|]-���Q`�ļ\��u�s�\i��Z�{ݏ�ͥѰ�M�>F�t�٭�o �oxյ�u��[�#B{��4A2L�����M�7�g�U�v1�Ik ��� �)6Z����Ņ6�<]3r	�n����"�P[�Z��ڬl�<ݺ�Y��*DfA՞��Ԧ�г
Twy����=�lU�@��j��f�B#��Ǜ�jγ�*�@2dm�=�2��)��9�&��-�%���x�T�K��r��;�^�?��RX      �   /   x�3�t�<�0W��ć�'&sqz?ܵ_�$���<.cN��=... 8�     