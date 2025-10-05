--
-- PostgreSQL database dump
--

\restrict wZOLO7AOYaft2BVeKbdXOV4fNIXTv5WKw5HrSIQBwD412Li1jefeFMLgm05Ez4i

-- Dumped from database version 16.10 (Debian 16.10-1.pgdg13+1)
-- Dumped by pg_dump version 16.10 (Debian 16.10-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: datosempresascompletar; Type: TABLE; Schema: public; Owner: Ingelab
--

CREATE TABLE public.datosempresascompletar (
    id_datosempresas integer NOT NULL,
    id_empresa integer,
    ubicacion character varying(100),
    sector character varying(50),
    tamano character varying(20),
    modalidad character varying(20),
    sitio_web character varying(150),
    descripcion_empresa text,
    tecnologia_clave text
);


ALTER TABLE public.datosempresascompletar OWNER TO "Ingelab";

--
-- Name: datosempresascompletar_id_datosempresas_seq; Type: SEQUENCE; Schema: public; Owner: Ingelab
--

CREATE SEQUENCE public.datosempresascompletar_id_datosempresas_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.datosempresascompletar_id_datosempresas_seq OWNER TO "Ingelab";

--
-- Name: datosempresascompletar_id_datosempresas_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Ingelab
--

ALTER SEQUENCE public.datosempresascompletar_id_datosempresas_seq OWNED BY public.datosempresascompletar.id_datosempresas;


--
-- Name: datosprofesionales; Type: TABLE; Schema: public; Owner: Ingelab
--

CREATE TABLE public.datosprofesionales (
    id_profesional integer NOT NULL,
    id_usuario integer NOT NULL,
    anios_experiencia integer,
    nivel_academico character varying(100),
    habilidades_tecnicas text,
    especializacion character varying(60),
    idiomas text,
    disponibilidad character varying(60)
);


ALTER TABLE public.datosprofesionales OWNER TO "Ingelab";

--
-- Name: datosprofesionales_id_profesional_seq; Type: SEQUENCE; Schema: public; Owner: Ingelab
--

CREATE SEQUENCE public.datosprofesionales_id_profesional_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.datosprofesionales_id_profesional_seq OWNER TO "Ingelab";

--
-- Name: datosprofesionales_id_profesional_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Ingelab
--

ALTER SEQUENCE public.datosprofesionales_id_profesional_seq OWNED BY public.datosprofesionales.id_profesional;


--
-- Name: empresas; Type: TABLE; Schema: public; Owner: Ingelab
--

CREATE TABLE public.empresas (
    id_empresa integer NOT NULL,
    nombre character varying(150) NOT NULL,
    nit character varying(25) NOT NULL,
    correo character varying(250) NOT NULL,
    "contrase├▒a" character varying(250) NOT NULL,
    telefono character varying(150) NOT NULL
);


ALTER TABLE public.empresas OWNER TO "Ingelab";

--
-- Name: empresas_id_empresa_seq; Type: SEQUENCE; Schema: public; Owner: Ingelab
--

CREATE SEQUENCE public.empresas_id_empresa_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.empresas_id_empresa_seq OWNER TO "Ingelab";

--
-- Name: empresas_id_empresa_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Ingelab
--

ALTER SEQUENCE public.empresas_id_empresa_seq OWNED BY public.empresas.id_empresa;


--
-- Name: ingenieros_contactados; Type: TABLE; Schema: public; Owner: Ingelab
--

CREATE TABLE public.ingenieros_contactados (
    id_contacto integer NOT NULL,
    id_usuario integer NOT NULL,
    oferta text,
    fecha_contacto timestamp without time zone DEFAULT now(),
    id_empresa integer,
    estado character varying(20) DEFAULT 'pendiente'::character varying,
    leido boolean DEFAULT false
);


ALTER TABLE public.ingenieros_contactados OWNER TO "Ingelab";

--
-- Name: ingenieros_contactados_id_contacto_seq; Type: SEQUENCE; Schema: public; Owner: Ingelab
--

CREATE SEQUENCE public.ingenieros_contactados_id_contacto_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ingenieros_contactados_id_contacto_seq OWNER TO "Ingelab";

--
-- Name: ingenieros_contactados_id_contacto_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Ingelab
--

ALTER SEQUENCE public.ingenieros_contactados_id_contacto_seq OWNED BY public.ingenieros_contactados.id_contacto;


--
-- Name: ingenieros_deseados; Type: TABLE; Schema: public; Owner: Ingelab
--

CREATE TABLE public.ingenieros_deseados (
    id_ingenierosdeseados integer NOT NULL,
    id_usuario integer NOT NULL,
    id_empresa integer,
    fecha_deseado timestamp without time zone DEFAULT now()
);


ALTER TABLE public.ingenieros_deseados OWNER TO "Ingelab";

--
-- Name: ingenieros_deseados_id_ingenierosdeseados_seq; Type: SEQUENCE; Schema: public; Owner: Ingelab
--

CREATE SEQUENCE public.ingenieros_deseados_id_ingenierosdeseados_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ingenieros_deseados_id_ingenierosdeseados_seq OWNER TO "Ingelab";

--
-- Name: ingenieros_deseados_id_ingenierosdeseados_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Ingelab
--

ALTER SEQUENCE public.ingenieros_deseados_id_ingenierosdeseados_seq OWNED BY public.ingenieros_deseados.id_ingenierosdeseados;


--
-- Name: notificaciones_empresa; Type: TABLE; Schema: public; Owner: Ingelab
--

CREATE TABLE public.notificaciones_empresa (
    id_notificacion integer NOT NULL,
    id_empresa integer NOT NULL,
    id_usuario integer,
    mensaje text NOT NULL,
    fecha timestamp without time zone DEFAULT now(),
    leido boolean DEFAULT false
);


ALTER TABLE public.notificaciones_empresa OWNER TO "Ingelab";

--
-- Name: notificaciones_empresa_id_notificacion_seq; Type: SEQUENCE; Schema: public; Owner: Ingelab
--

CREATE SEQUENCE public.notificaciones_empresa_id_notificacion_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.notificaciones_empresa_id_notificacion_seq OWNER TO "Ingelab";

--
-- Name: notificaciones_empresa_id_notificacion_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Ingelab
--

ALTER SEQUENCE public.notificaciones_empresa_id_notificacion_seq OWNED BY public.notificaciones_empresa.id_notificacion;


--
-- Name: postingeniero; Type: TABLE; Schema: public; Owner: Ingelab
--

CREATE TABLE public.postingeniero (
    id_post integer NOT NULL,
    id_usuario integer,
    contenido text NOT NULL,
    fecha_public timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    tipo_contenido character varying(20),
    fijado boolean DEFAULT false
);


ALTER TABLE public.postingeniero OWNER TO "Ingelab";

--
-- Name: postingeniero_id_post_seq; Type: SEQUENCE; Schema: public; Owner: Ingelab
--

CREATE SEQUENCE public.postingeniero_id_post_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.postingeniero_id_post_seq OWNER TO "Ingelab";

--
-- Name: postingeniero_id_post_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Ingelab
--

ALTER SEQUENCE public.postingeniero_id_post_seq OWNED BY public.postingeniero.id_post;


--
-- Name: usuarios; Type: TABLE; Schema: public; Owner: Ingelab
--

CREATE TABLE public.usuarios (
    id_usuario integer NOT NULL,
    nombre character varying(65) NOT NULL,
    apellidos character varying(65) NOT NULL,
    tipo_documento character varying(40) NOT NULL,
    numero_documento character varying(50) NOT NULL,
    correo character varying(200) NOT NULL,
    "contrase├▒a" character varying(255) NOT NULL,
    fecha_nacimiento date,
    telefono character varying(20)
);


ALTER TABLE public.usuarios OWNER TO "Ingelab";

--
-- Name: usuarios_id_usuario_seq; Type: SEQUENCE; Schema: public; Owner: Ingelab
--

CREATE SEQUENCE public.usuarios_id_usuario_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.usuarios_id_usuario_seq OWNER TO "Ingelab";

--
-- Name: usuarios_id_usuario_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Ingelab
--

ALTER SEQUENCE public.usuarios_id_usuario_seq OWNED BY public.usuarios.id_usuario;


--
-- Name: datosempresascompletar id_datosempresas; Type: DEFAULT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.datosempresascompletar ALTER COLUMN id_datosempresas SET DEFAULT nextval('public.datosempresascompletar_id_datosempresas_seq'::regclass);


--
-- Name: datosprofesionales id_profesional; Type: DEFAULT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.datosprofesionales ALTER COLUMN id_profesional SET DEFAULT nextval('public.datosprofesionales_id_profesional_seq'::regclass);


--
-- Name: empresas id_empresa; Type: DEFAULT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.empresas ALTER COLUMN id_empresa SET DEFAULT nextval('public.empresas_id_empresa_seq'::regclass);


--
-- Name: ingenieros_contactados id_contacto; Type: DEFAULT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.ingenieros_contactados ALTER COLUMN id_contacto SET DEFAULT nextval('public.ingenieros_contactados_id_contacto_seq'::regclass);


--
-- Name: ingenieros_deseados id_ingenierosdeseados; Type: DEFAULT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.ingenieros_deseados ALTER COLUMN id_ingenierosdeseados SET DEFAULT nextval('public.ingenieros_deseados_id_ingenierosdeseados_seq'::regclass);


--
-- Name: notificaciones_empresa id_notificacion; Type: DEFAULT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.notificaciones_empresa ALTER COLUMN id_notificacion SET DEFAULT nextval('public.notificaciones_empresa_id_notificacion_seq'::regclass);


--
-- Name: postingeniero id_post; Type: DEFAULT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.postingeniero ALTER COLUMN id_post SET DEFAULT nextval('public.postingeniero_id_post_seq'::regclass);


--
-- Name: usuarios id_usuario; Type: DEFAULT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.usuarios ALTER COLUMN id_usuario SET DEFAULT nextval('public.usuarios_id_usuario_seq'::regclass);


--
-- Data for Name: datosempresascompletar; Type: TABLE DATA; Schema: public; Owner: Ingelab
--

COPY public.datosempresascompletar (id_datosempresas, id_empresa, ubicacion, sector, tamano, modalidad, sitio_web, descripcion_empresa, tecnologia_clave) FROM stdin;
57	74	Medellin	construccion	11-50	remoto	https://tracker.gg/valorant/profile/riot/DarKSP%23LAN/overview	hola	react
\.


--
-- Data for Name: datosprofesionales; Type: TABLE DATA; Schema: public; Owner: Ingelab
--

COPY public.datosprofesionales (id_profesional, id_usuario, anios_experiencia, nivel_academico, habilidades_tecnicas, especializacion, idiomas, disponibilidad) FROM stdin;
37	119	18	bachillerato	C#,Azure	Infraestructura	Espa├▒ol,ingles	full-time
38	120	20	bachillerato	java, backend	Desarrollo de web	Frances	part-time
39	121	5	certificaciones	Azure,AWS	Infraestructura	ingles	part-time
40	122	15	certificaciones	 SQL,Python	Ciberseguridad	 ingles	open
41	123	16	licenciatura	Algoritmos,java,c#,AWS	Inteligencia artificial	ingles,espa├▒ol	part-time
42	124	20	maestria	Backend,.NET,C#,java,AWS	Desarrollo de web	Ingles	part-time
\.


--
-- Data for Name: empresas; Type: TABLE DATA; Schema: public; Owner: Ingelab
--

COPY public.empresas (id_empresa, nombre, nit, correo, "contrase├▒a", telefono) FROM stdin;
74	Ingelab	412431414	ingelab@gmail.com	$2a$11$x.6pC6wcxRkM0PTvr4aBoOmu5vja96WUm8hNQVRgjDT0j9AsklYJa	3123213213
\.


--
-- Data for Name: ingenieros_contactados; Type: TABLE DATA; Schema: public; Owner: Ingelab
--

COPY public.ingenieros_contactados (id_contacto, id_usuario, oferta, fecha_contacto, id_empresa, estado, leido) FROM stdin;
40	119	te propongo trabajar con nosotros	2025-10-02 22:06:57.352685	74	aceptada	t
42	124	oe	2025-10-02 22:33:48.708889	74	aceptada	t
43	123	quiero que trabajes pa nosotros	2025-10-02 22:34:30.029429	74	rechazada	t
\.


--
-- Data for Name: ingenieros_deseados; Type: TABLE DATA; Schema: public; Owner: Ingelab
--

COPY public.ingenieros_deseados (id_ingenierosdeseados, id_usuario, id_empresa, fecha_deseado) FROM stdin;
34	121	74	2025-10-02 22:21:20.436889
\.


--
-- Data for Name: notificaciones_empresa; Type: TABLE DATA; Schema: public; Owner: Ingelab
--

COPY public.notificaciones_empresa (id_notificacion, id_empresa, id_usuario, mensaje, fecha, leido) FROM stdin;
22	74	119	El ingeniero acept├│ tu oferta	2025-10-02 22:07:27.742541	f
23	74	120	El ingeniero rechazo tu oferta	2025-10-02 22:21:03.797346	f
24	74	124	El ingeniero acept├│ tu oferta	2025-10-02 22:33:54.18881	f
25	74	123	El ingeniero rechazo tu oferta	2025-10-02 22:34:57.492936	f
\.


--
-- Data for Name: postingeniero; Type: TABLE DATA; Schema: public; Owner: Ingelab
--

COPY public.postingeniero (id_post, id_usuario, contenido, fecha_public, tipo_contenido, fijado) FROM stdin;
113	122	mi proyecto consiste en una pagina pa buscar tarbajo actualizad	2025-10-02 17:23:51.773244	texto	t
115	123	mi proyecto consiste en una pagina para el fantasy 	2025-10-02 17:28:15.682512	texto	f
116	123	{"texto":null,"codigo":"using System;\\r\\n\\r\\nclass Program\\r\\n{\\r\\n    static void Main()\\r\\n    {\\r\\n        Console.Write(\\"Ingrese un n├║mero: \\");\\r\\n        int numero = int.Parse(Console.ReadLine());\\r\\n\\r\\n        if (numero % 2 == 0)\\r\\n        {\\r\\n            Console.WriteLine($\\"El n├║mero {numero} es PAR.\\");\\r\\n        }\\r\\n        else\\r\\n        {\\r\\n            Console.WriteLine($\\"El n├║mero {numero} es IMPAR.\\");\\r\\n        }\\r\\n    }\\r\\n}\\r\\n"}	2025-10-02 17:28:46.466843	csharp	f
117	124	hi this is my first project	2025-10-02 17:33:19.397356	texto	f
108	119	este es mi sgundo post\r\n	2025-10-02 17:08:05.566531	texto	f
107	119	este es mi rpimer post papi jijiji actualizado	2025-10-02 17:07:56.688328	texto	t
110	121	mi primero proyecto actualizado	2025-10-02 17:18:16.52582	texto	f
111	120	{"texto":null,"codigo":"using System;\\r\\n\\r\\nclass Program\\r\\n{\\r\\n    static void Main()\\r\\n    {\\r\\n        Console.WriteLine(\\"Hello, World!\\");\\r\\n    }\\r\\n}\\r\\n"}	2025-10-02 17:19:18.408205	csharp	t
114	122	mi segundo proyecto es un bomberman	2025-10-02 17:24:10.370814	texto	f
\.


--
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: Ingelab
--

COPY public.usuarios (id_usuario, nombre, apellidos, tipo_documento, numero_documento, correo, "contrase├▒a", fecha_nacimiento, telefono) FROM stdin;
119	Samuel	Parra 	Cedula	42143124314	samuelparr34@gmail.com	$2a$11$fvXO/zoMiOyv.7GTpK5MXOV0hqieYY5dgP9xwRMXkAypGLyo8yCnW	1994-10-25	3132284538
120	camila	Olivares Restrepo	Cedula	41412343141	mcamiolivares@gmail.com	$2a$11$Uxtpoq2ikCQT2BECvQgZduq4gLAeE3tqssmF0r9fitsp9ouPzqisS	1985-05-02	3124354545
121	Pablo	Zuluaga	Cedula	414314314	pzuluaga@gmail.com	$2a$11$XITYppVQDHx/YEUL4a99n.luG8A7BWwwJmUun3yrSO9nqVso/J0Ia	2025-09-09	3143143143
122	Juan jose	Rave benitez	Cedula	4123412441	juan@gmail.com	$2a$11$AgpxP3i9kzqpF70Xd0LXZ.2dy9QGvX1uel5dmo6ZwEjSdX75Yc/gy	2025-06-19	5654364354
123	lionel	messi cuccitini	Cedula	421423141414	messi123@gmail.com	$2a$11$gyqcScXxYPXVAhEVpvECtO7qp8VipZQJqe2dP9/Jr/X.0Js2e6Gq6	1991-09-02	4143143123
124	Sebastian	Quijano	Cedula	321342141	sebastianQuijano@gmail.com	$2a$11$CowJsrvfcM9C5aN0q3sMqeTf.x5oi4byp1ap6/gBe6QNcnx2XXGn.	1996-09-23	1435454545
\.


--
-- Name: datosempresascompletar_id_datosempresas_seq; Type: SEQUENCE SET; Schema: public; Owner: Ingelab
--

SELECT pg_catalog.setval('public.datosempresascompletar_id_datosempresas_seq', 57, true);


--
-- Name: datosprofesionales_id_profesional_seq; Type: SEQUENCE SET; Schema: public; Owner: Ingelab
--

SELECT pg_catalog.setval('public.datosprofesionales_id_profesional_seq', 42, true);


--
-- Name: empresas_id_empresa_seq; Type: SEQUENCE SET; Schema: public; Owner: Ingelab
--

SELECT pg_catalog.setval('public.empresas_id_empresa_seq', 74, true);


--
-- Name: ingenieros_contactados_id_contacto_seq; Type: SEQUENCE SET; Schema: public; Owner: Ingelab
--

SELECT pg_catalog.setval('public.ingenieros_contactados_id_contacto_seq', 43, true);


--
-- Name: ingenieros_deseados_id_ingenierosdeseados_seq; Type: SEQUENCE SET; Schema: public; Owner: Ingelab
--

SELECT pg_catalog.setval('public.ingenieros_deseados_id_ingenierosdeseados_seq', 34, true);


--
-- Name: notificaciones_empresa_id_notificacion_seq; Type: SEQUENCE SET; Schema: public; Owner: Ingelab
--

SELECT pg_catalog.setval('public.notificaciones_empresa_id_notificacion_seq', 25, true);


--
-- Name: postingeniero_id_post_seq; Type: SEQUENCE SET; Schema: public; Owner: Ingelab
--

SELECT pg_catalog.setval('public.postingeniero_id_post_seq', 117, true);


--
-- Name: usuarios_id_usuario_seq; Type: SEQUENCE SET; Schema: public; Owner: Ingelab
--

SELECT pg_catalog.setval('public.usuarios_id_usuario_seq', 124, true);


--
-- Name: datosempresascompletar datosempresascompletar_pkey; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.datosempresascompletar
    ADD CONSTRAINT datosempresascompletar_pkey PRIMARY KEY (id_datosempresas);


--
-- Name: datosprofesionales datosprofesionales_id_usuario_key; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.datosprofesionales
    ADD CONSTRAINT datosprofesionales_id_usuario_key UNIQUE (id_usuario);


--
-- Name: datosprofesionales datosprofesionales_pkey; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.datosprofesionales
    ADD CONSTRAINT datosprofesionales_pkey PRIMARY KEY (id_profesional);


--
-- Name: empresas empresas_correo_key; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.empresas
    ADD CONSTRAINT empresas_correo_key UNIQUE (correo);


--
-- Name: empresas empresas_nit_key; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.empresas
    ADD CONSTRAINT empresas_nit_key UNIQUE (nit);


--
-- Name: empresas empresas_pkey; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.empresas
    ADD CONSTRAINT empresas_pkey PRIMARY KEY (id_empresa);


--
-- Name: ingenieros_contactados ingenieros_contactados_pkey; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.ingenieros_contactados
    ADD CONSTRAINT ingenieros_contactados_pkey PRIMARY KEY (id_contacto);


--
-- Name: ingenieros_deseados ingenieros_deseados_pkey; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.ingenieros_deseados
    ADD CONSTRAINT ingenieros_deseados_pkey PRIMARY KEY (id_ingenierosdeseados);


--
-- Name: notificaciones_empresa notificaciones_empresa_pkey; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.notificaciones_empresa
    ADD CONSTRAINT notificaciones_empresa_pkey PRIMARY KEY (id_notificacion);


--
-- Name: postingeniero postingeniero_pkey; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.postingeniero
    ADD CONSTRAINT postingeniero_pkey PRIMARY KEY (id_post);


--
-- Name: usuarios usuarios_correo_key; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_correo_key UNIQUE (correo);


--
-- Name: usuarios usuarios_numero_documento_key; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_numero_documento_key UNIQUE (numero_documento);


--
-- Name: usuarios usuarios_pkey; Type: CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id_usuario);


--
-- Name: datosempresascompletar datosempresascompletar_id_empresa_fkey; Type: FK CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.datosempresascompletar
    ADD CONSTRAINT datosempresascompletar_id_empresa_fkey FOREIGN KEY (id_empresa) REFERENCES public.empresas(id_empresa);


--
-- Name: ingenieros_contactados fk_empresa; Type: FK CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.ingenieros_contactados
    ADD CONSTRAINT fk_empresa FOREIGN KEY (id_empresa) REFERENCES public.empresas(id_empresa);


--
-- Name: ingenieros_deseados fk_empresa; Type: FK CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.ingenieros_deseados
    ADD CONSTRAINT fk_empresa FOREIGN KEY (id_empresa) REFERENCES public.empresas(id_empresa);


--
-- Name: ingenieros_contactados fk_usuario; Type: FK CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.ingenieros_contactados
    ADD CONSTRAINT fk_usuario FOREIGN KEY (id_usuario) REFERENCES public.usuarios(id_usuario) ON DELETE CASCADE;


--
-- Name: ingenieros_deseados fk_usuario; Type: FK CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.ingenieros_deseados
    ADD CONSTRAINT fk_usuario FOREIGN KEY (id_usuario) REFERENCES public.usuarios(id_usuario) ON DELETE CASCADE;


--
-- Name: datosprofesionales id_usuario; Type: FK CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.datosprofesionales
    ADD CONSTRAINT id_usuario FOREIGN KEY (id_usuario) REFERENCES public.usuarios(id_usuario);


--
-- Name: postingeniero postingeniero_id_usuario_fkey; Type: FK CONSTRAINT; Schema: public; Owner: Ingelab
--

ALTER TABLE ONLY public.postingeniero
    ADD CONSTRAINT postingeniero_id_usuario_fkey FOREIGN KEY (id_usuario) REFERENCES public.usuarios(id_usuario);


--
-- PostgreSQL database dump complete
--

\unrestrict wZOLO7AOYaft2BVeKbdXOV4fNIXTv5WKw5HrSIQBwD412Li1jefeFMLgm05Ez4i

