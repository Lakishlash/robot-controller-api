--
-- PostgreSQL database dump
--

-- Dumped from database version 17.4
-- Dumped by pg_dump version 17.4 (Homebrew)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
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
-- Name: map; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.map (
    id integer NOT NULL,
    rows integer NOT NULL,
    columns integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(800),
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL,
    issquare boolean GENERATED ALWAYS AS ((rows = columns)) STORED
);


ALTER TABLE public.map OWNER TO postgres;

--
-- Name: map_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.map_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.map_id_seq OWNER TO postgres;

--
-- Name: map_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.map_id_seq OWNED BY public.map.id;


--
-- Name: robotcommand; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.robotcommand (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(800),
    ismovecommand boolean NOT NULL,
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL
);


ALTER TABLE public.robotcommand OWNER TO postgres;

--
-- Name: robotcommand_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.robotcommand_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.robotcommand_id_seq OWNER TO postgres;

--
-- Name: robotcommand_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.robotcommand_id_seq OWNED BY public.robotcommand.id;


--
-- Name: map id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.map ALTER COLUMN id SET DEFAULT nextval('public.map_id_seq'::regclass);


--
-- Name: robotcommand id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.robotcommand ALTER COLUMN id SET DEFAULT nextval('public.robotcommand_id_seq'::regclass);


--
-- Name: map map_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.map
    ADD CONSTRAINT map_pkey PRIMARY KEY (id);


--
-- Name: robotcommand robotcommand_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.robotcommand
    ADD CONSTRAINT robotcommand_pkey PRIMARY KEY (id);


--
-- PostgreSQL database dump complete
--

