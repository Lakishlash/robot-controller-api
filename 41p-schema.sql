-- RobotCommand table + sequence
CREATE TABLE IF NOT EXISTS public.robotcommand (
  id            integer     NOT NULL,
  name          varchar(50) NOT NULL,
  description   varchar(800),
  ismovecommand boolean     NOT NULL,
  createddate   timestamp   NOT NULL,
  modifieddate  timestamp   NOT NULL,
  CONSTRAINT robotcommand_pkey PRIMARY KEY (id)
);

CREATE SEQUENCE IF NOT EXISTS public.robotcommand_id_seq
  AS integer START WITH 1 INCREMENT BY 1 CACHE 1;

ALTER TABLE ONLY public.robotcommand
  ALTER COLUMN id SET DEFAULT nextval('public.robotcommand_id_seq'::regclass);

-- Map table + sequence (if you still need to recreate it)
CREATE TABLE IF NOT EXISTS public.map (
  id           integer     NOT NULL,
  rows         integer     NOT NULL,
  columns      integer     NOT NULL,
  name         varchar(50) NOT NULL,
  description  varchar(800),
  createddate  timestamp   NOT NULL,
  modifieddate timestamp   NOT NULL,
  issquare     boolean     GENERATED ALWAYS AS ((rows = columns)) STORED,
  CONSTRAINT map_pkey PRIMARY KEY (id)
);

CREATE SEQUENCE IF NOT EXISTS public.map_id_seq
  AS integer START WITH 1 INCREMENT BY 1 CACHE 1;

ALTER TABLE ONLY public.map
  ALTER COLUMN id SET DEFAULT nextval('public.map_id_seq'::regclass);
