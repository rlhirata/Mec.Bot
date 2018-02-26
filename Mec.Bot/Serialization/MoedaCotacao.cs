using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mec.Bot.Serialization
{
    public class MoedaCotacao
    {
        public bool status { get; set; }
        public Valores valores { get; set; }
    }

    public class Valores
    {
        public USD USD { get; set; }
        public EUR EUR { get; set; }
        public ARS ARS { get; set; }
        public GBP GBP { get; set; }
        public BTC BTC { get; set; }
    }

    public class USD
    {
        public string nome { get; set; }
        public double valor { get; set; }
        public int ultima_consulta { get; set; }
        public string fonte { get; set; }
    }

    public class EUR
    {
        public string nome { get; set; }
        public double valor { get; set; }
        public int ultima_consulta { get; set; }
        public string fonte { get; set; }
    }

    public class ARS
    {
        public string nome { get; set; }
        public double valor { get; set; }
        public int ultima_consulta { get; set; }
        public string fonte { get; set; }
    }

    public class GBP
    {
        public string nome { get; set; }
        public double valor { get; set; }
        public int ultima_consulta { get; set; }
        public string fonte { get; set; }
    }

    public class BTC
    {
        public string nome { get; set; }
        public double valor { get; set; }
        public int ultima_consulta { get; set; }
        public string fonte { get; set; }
    }
}