using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        private static ClienteModel cli;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else if (!ValidaCPF(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("CPF inválido!");
            }
            else if (bo.VerificarExistencia(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("Esse CPF já esta cadastrado!");
            }
            else
            {
                model.Id = bo.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });
                return Json("Cadastro efetuado com sucesso");
            }
        }
        
        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                        from error in item.Errors
                                        select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else if (!ValidaCPF(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("CPF inválido!");
            }
            else if (cli.CPF != model.CPF && bo.VerificarExistencia(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("Esse CPF já esta cadastrado!");
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                return Json("Cadastro alterado com sucesso");
            }                
            
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };
            }
            cli = model;
            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public static bool ValidaCPF(string cpf)
        {
            int[] verificador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] verificardor2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string parteCPF;
            string digitoFinal;
            int somador;
            int restante;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            switch (cpf)
            {
                case "11111111111":
                    return false;
                case "00000000000":
                    return false;
                case "22222222222":
                    return false;
                case "33333333333":
                    return false;
                case "44444444444":
                    return false;
                case "55555555555":
                    return false;
                case "66666666666":
                    return false;
                case "77777777777":
                    return false;
                case "88888888888":
                    return false;
                case "99999999999":
                    return false;
            }

            parteCPF = cpf.Substring(0, 9);
            somador = 0;

            for (int i = 0; i < 9; i++)
                somador += int.Parse(parteCPF[i].ToString()) * verificador1[i];
            restante = somador % 11;
            if (restante < 2)
                restante = 0;
            else
                restante = 11 - restante;
            digitoFinal = restante.ToString();
            parteCPF = parteCPF + digitoFinal;
            somador = 0;
            for (int i = 0; i < 10; i++)
                somador += int.Parse(parteCPF[i].ToString()) * verificardor2[i];
            restante = somador % 11;
            if (restante < 2)
                restante = 0;
            else
                restante = 11 - restante;
            digitoFinal += restante.ToString();
            return cpf.EndsWith(digitoFinal);
        }

    }
}