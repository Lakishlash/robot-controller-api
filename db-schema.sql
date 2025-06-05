--
-- PostgreSQL database dump
--

-- Dumped from database version 17.4 (Homebrew)
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

--
-- Name: map_id_seq; Type: SEQUENCE; Schema: public; Owner: lakishwijewardene
--

CREATE SEQUENCE public.map_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.map_id_seq OWNER TO lakishwijewardene;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: map; Type: TABLE; Schema: public; Owner: lakishwijewardene
--

CREATE TABLE public.map (
    id integer DEFAULT nextval('public.map_id_seq'::regclass) NOT NULL,
    rows integer NOT NULL,
    columns integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(800),
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL,
    issquare boolean GENERATED ALWAYS AS ((rows = columns)) STORED
);


ALTER TABLE public.map OWNER TO lakishwijewardene;

--
-- Name: robotcommand_id_seq; Type: SEQUENCE; Schema: public; Owner: lakishwijewardene
--

CREATE SEQUENCE public.robotcommand_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.robotcommand_id_seq OWNER TO lakishwijewardene;

--
-- Name: robotcommand; Type: TABLE; Schema: public; Owner: lakishwijewardene
--

CREATE TABLE public.robotcommand (
    id integer DEFAULT nextval('public.robotcommand_id_seq'::regclass) NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(800),
    ismovecommand boolean NOT NULL,
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL
);


ALTER TABLE public.robotcommand OWNER TO lakishwijewardene;

--
-- Name: user; Type: TABLE; Schema: public; Owner: lakishwijewardene
--

CREATE TABLE public."user" (
    id integer NOT NULL,
    email text NOT NULL,
    first_name text NOT NULL,
    last_name text NOT NULL,
    password_hash text NOT NULL,
    role text NOT NULL,
    description text,
    created_date timestamp without time zone DEFAULT now() NOT NULL,
    modified_date timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public."user" OWNER TO lakishwijewardene;

--
-- Name: user_id_seq; Type: SEQUENCE; Schema: public; Owner: lakishwijewardene
--

CREATE SEQUENCE public.user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.user_id_seq OWNER TO lakishwijewardene;

--
-- Name: user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: lakishwijewardene
--

ALTER SEQUENCE public.user_id_seq OWNED BY public."user".id;


--
-- Name: user id; Type: DEFAULT; Schema: public; Owner: lakishwijewardene
--

ALTER TABLE ONLY public."user" ALTER COLUMN id SET DEFAULT nextval('public.user_id_seq'::regclass);


--
-- Name: map map_pkey; Type: CONSTRAINT; Schema: public; Owner: lakishwijewardene
--

ALTER TABLE ONLY public.map
    ADD CONSTRAINT map_pkey PRIMARY KEY (id);


--
-- Name: robotcommand robotcommand_pkey; Type: CONSTRAINT; Schema: public; Owner: lakishwijewardene
--

ALTER TABLE ONLY public.robotcommand
    ADD CONSTRAINT robotcommand_pkey PRIMARY KEY (id);


--
-- Name: user user_email_key; Type: CONSTRAINT; Schema: public; Owner: lakishwijewardene
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_email_key UNIQUE (email);


--
-- Name: user user_pkey; Type: CONSTRAINT; Schema: public; Owner: lakishwijewardene
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_pkey PRIMARY KEY (id);


--
-- PostgreSQL database dump complete
--

