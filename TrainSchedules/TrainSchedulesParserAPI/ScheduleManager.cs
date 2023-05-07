using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrainSchedulesParserAPI
{
    internal static class ScheduleManager
    {
        public static ScheduleResponse Search(ScheduleRequest request)
        {
            List<NodesComboioTabelsPartidasChegada> horarios = new();

            string url = BuildURL(request);

            Console.Write(url);

            string response = string.Empty;

            if (!string.IsNullOrEmpty(url))
            {
                response = GetHTTPResponse(url);

                horarios = ProcessHorarios(response);
            }

            if (horarios.Count > 0)
            {
                ScheduleResponse scheduleResponse = new();
                scheduleResponse.Nexttrain = horarios.First().DataHoraPartidaChegada;
                scheduleResponse.NextTrainStatus = string.IsNullOrWhiteSpace(horarios.First().Observacoes) ? "OK" : horarios.First().Observacoes;
                scheduleResponse.Next3Trains = horarios.Take(3).Select(x => x.DataHoraPartidaChegada).ToList();
                scheduleResponse.Next3TrainsFull = horarios.Take(3).Select(x => x.DataHoraPartidaChegada + " " + (string.IsNullOrWhiteSpace(x.Observacoes) ? "OK" : x.Observacoes)).ToList();

                return scheduleResponse;
            }
            else
            {
                return null;
            }
        }

        public static string GetHTTPResponse(string uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(Convert.ToString(e.Message));
                return string.Empty;
            }
        }

        public static string BuildURL(ScheduleRequest request)
        {
            string stationID = GetStationId(request.Station);

            string startDate = request.RequestTime.ToString("yyyy-MM-dd HH:mm");
            string endDate = request.RequestTime.AddHours(1).ToString("yyyy-MM-dd HH:mm");

            string dateURL = startDate + "/" + endDate;

            StringBuilder returnURL = new();
            returnURL.Append("https://servicos.infraestruturasdeportugal.pt/negocios-e-servicos/partidas-chegadas/");
            returnURL.Append(stationID);
            returnURL.Append("/" + dateURL);
            returnURL.Append("/INTERNACIONAL,%20ALFA,%20IC,%20IR,%20REGIONAL,%20URB%7CSUBUR,%20ESPECIAL");

            return returnURL.ToString();
        }

        private static string GetStationName(int stationKey)
        {
            return GetStations().FirstOrDefault(x => x.Value == stationKey).Key;
        }

        private static string GetStationId(string stationName)
        {
            return Convert.ToString(GetStations().FirstOrDefault(x => x.Key == stationName).Value);
        }

        private static List<NodesComboioTabelsPartidasChegada> ProcessHorarios(string response)
        {
            List<NodesComboioTabelsPartidasChegada> horarios = new List<NodesComboioTabelsPartidasChegada>();

            if (!string.IsNullOrEmpty(response))
            {
                Response partidasResponse = JsonConvert.DeserializeObject<Root>(response).Response.First();
                horarios = partidasResponse.NodesComboioTabelsPartidasChegadas;
            }

            return horarios;
        }

        private static void PrintValues(List<NodesComboioTabelsPartidasChegada> horarios)
        {
            foreach (var horario in horarios)
            {
                string estacaoOrigem = GetStationName(horario.EstacaoOrigem);
                string estacaoDestino = GetStationName(horario.EstacaoDestino);

                Console.WriteLine(estacaoOrigem + " > " + estacaoDestino + " " + horario.DataHoraPartidaChegada + " " + (horario.Observacoes.Contains("Suprimido") ? "Suprimido" : "OK"));
            }
        }

        public static Dictionary<string, int> GetStations()
        {
            try
            {
                return new Dictionary<string, int>
                {
                    {"Abrantes", 9452001}, {"Abreiro", 9415289}, {"Abrunhosa", 9448124}, {"Adémia", 9436046}, {"Afife", 9418119}, {"Agolada", 9480283}, {"Agualva-Cacém", 9461002}, {"Águas De Moura", 9492007}, {"Águas Santas", 9403046},
                    {"Aguda", 9439057}, {"Águeda", 9442218}, {"Aguieira", 9442267}, {"Aguim", 9437093}, {"Alberg. Dos Doze", 9434439}, {"Albergaria Nova", 9444495}, {"Albergaria Velha", 9444552}, {"Albufeira", 9478063}, {"Alcácer Do Sal", 9492338},
                    {"Alcáçovas", 9474120}, {"Alcaide", 9453504}, {"Alcainça-Moinhos", 9462190}, {"Alcains", 9453140}, {"Alcântara-Mar", 9469039}, {"Alcântara-Terra", 9467025}, {"Alcantarilha", 9490092}, {"Alcaria", 9453629}, {"Aldeia", 9449411},
                    {"Alegria", 9411064}, {"Alfarelos", 9435006}, {"Alferrarede", 9452068}, {"Algés", 9469088}, {"Algoz", 9490050}, {"Algueirão", 9461069}, {"Alhadas (A)", 9445435}, {"Alhandra", 9431237}, {"Alhos Vedros", 9495075},
                    {"Almancil", 9478253}, {"Almourol", 9451102}, {"Alpedrinha", 9453355}, {"Alvações (A)", 9414084}, {"Alvalade", 9493096}, {"Alvarães", 9406338}, {"Alvega-Ortiga", 9452209}, {"Alverca", 9431187}, {"Alvito", 9474351},
                    {"Amadora", 9460087}, {"Amarante", 9413136}, {"Ameal", 9435097}, {"Ameixial", 9483535}, {"Amieira", 9464006}, {"Amoreiras-Odemir", 9477099}, {"Âncora-Praia", 9418150}, {"Aranha (A)", 9483378}, {"Arazede (A)", 9445245},
                    {"Aregos", 9409191}, {"Areia-Darque", 9406395}, {"Arentim", 9429066}, {"Areosa", 9418044}, {"Aroeira", 9473486}, {"Arrifana", 9444248}, {"Arronches", 9457174}, {"Arroteia (A)", 9421055}, {"Assumar", 9457117},
                    {"Avanca", 9438216}, {"Aveiro", 9438000}, {"Aveiro-Vouga", 9442002}, {"Aveleda", 9429108}, {"Azambuja", 9433001}, {"Azaruja (Ad)", 9483204}, {"Azi.Barros-A", 9492775}, {"Azin. Dos Barros", 9492684}, {"Azurva", 9442051},
                    {"Bagaúste", 9410058}, {"Baraçal", 9448454}, {"Barca Da Amieira", 9452415}, {"Barcelos", 9406122}, {"Barqueiros", 9409324}, {"Barquinha", 9451045}, {"Barr. De Belver", 9452241}, {"Barreiro", 9495000}, {"Barreiro-A", 9495026},
                    {"Barrimau", 9405058}, {"Barroselas", 9406304}, {"Bebedouro (A)", 9445278}, {"Beja", 9475002}, {"Belém", 9469054}, {"Belmonte-Manteig", 9454197}, {"Belver", 9452282}, {"Bemposta", 9455129}, {"Bencanta", 9435170},
                    {"Benespera", 9454338}, {"Benfica", 9460046}, {"Benquerenças", 9452878}, {"Bicanho", 9463974}, {"Bif. De Lares", 9464022}, {"Bobadela", 9431070}, {"Boliqueime", 9478147}, {"Bom João", 9473031}, {"Bombarral", 9462703},
                    {"Bombel", 9471225}, {"Bouro", 9463081}, {"Braço De Prata", 9431005}, {"Braga", 9429157}, {"Branca", 9444461}, {"Brunheda (A)", 9415222}, {"Bustelo", 9408334}, {"Bxa. Da Banheira", 9495059}, {"C. De Cucujães", 9444297},
                    {"Cabanões", 9442150}, {"Cabeço De Vide (A)", 9486504}, {"Cabeda", 9408029}, {"Cacela", 9473452}, {"Cachão", 9415420}, {"Cachofarra", 9468171}, {"Cacia", 9438075}, {"Caíde", 9408383}, {"Cais Do Sodré", 9469005},
                    {"Caldas Da Rainha", 9463008}, {"Caldas De Moledo", 9409399}, {"Camarão", 9462661}, {"Caminha", 9418242}, {"Campolide", 9460004}, {"Campolide-A", 9467033}, {"Campo-Serra", 9463040}, {"Canal Caveira", 9492650}, {"Canas-Felgueiras", 9446599},
                    {"Canelas", 9438117}, {"Canha", 9480580}, {"Caniços", 9428100}, {"Cantanhede", 9445153}, {"Carapecos", 9406197}, {"Carcavelos", 9469187}, {"Cardiais (A)", 9486132}, {"Caria", 9454148}, {"Carrascal-Delong", 9440030},
                    {"Carrazedo (A)", 9414159}, {"Carreço", 9418085}, {"Carregado", 9431336}, {"Carregal Do Sal", 9446482}, {"Carreira", 9406056}, {"Carriço", 9463834}, {"Carv. Da Portela", 9442309}, {"Carvalha", 9418416}, {"Carvalhos De Fig", 9440113},
                    {"Carvalhosas", 9441079}, {"Carv-Maceda", 9438356}, {"Casa Branca", 9474005}, {"Casais", 9435147}, {"Casal (A)", 9445229}, {"Casal Do Álvaro", 9442168}, {"Cascais", 9469260}, {"Castanh.Ribatejo", 9431310}, {"Castanheiro (A)", 9415081},
                    {"Castelejo", 9446409}, {"Castelo Branco", 9453009}, {"Castelo Mendo", 9449312}, {"Castelo Novo", 9453314}, {"Castelo Vide", 9458503}, {"Castro Marim", 9473502}, {"Castro Verde-Almodóvar", 9476000}, {"Cavaco", 9444172}, {"Caxarias", 9434330},
                    {"Caxias", 9469120}, {"Ceira", 9441103}, {"Cela", 9463214}, {"Celorico Beira", 9448405}, {"Cerdeira", 9449205}, {"Cête", 9408227}, {"Chança", 9456101}, {"Chelas", 9466027}, {"Codeçais (A)", 9415263},
                    {"Coimbra", 9441020}, {"Coimbra-B", 9436004}, {"Coimbra-Parque", 9441038}, {"Coimbrões", 9439149}, {"Coina", 9417236}, {"Comenda (A)", 9483022}, {"Conceição", 9473379}, {"Conraria", 9441095}, {"Contenças", 9448066},
                    {"Contumil", 9403004}, {"Cordinhã (A)", 9445112}, {"Corgo", 9414019}, {"Corroios", 9417137}, {"Cortegaça", 9438372}, {"Coruche", 9480325}, {"Costeira (A)", 9445393}, {"Couto Cambeses", 9429033}, {"Coutos (A)", 9486462},
                    {"Covas", 9428290}, {"Covelinhas", 9410090}, {"Covilhã", 9454007}, {"Crato", 9456267}, {"Cruz Quebrada", 9469104}, {"Cruzeiro (A)", 9414183}, {"Cuba", 9474476}, {"Cuca", 9428209}, {"Cunheira", 9458131},
                    {"Curia", 9437119}, {"Curvaceiras", 9440063}, {"Dagorda-Peniche", 9462802}, {"Darque", 9406387}, {"Dois Portos", 9462380}, {"Donas", 9453520}, {"Durrães", 9406262}, {"Eirol", 9442119}, {"Eixo", 9442077},
                    {"Elvas", 9457497}, {"Entrecampos", 9466050}, {"Entrecampos-Poen", 9466068}, {"Entroncamento", 9434009}, {"Enxofães (A)", 9445088}, {"Ermesinde", 9404002}, {"Ermida", 9409258}, {"Ermidas-Sado", 9493005}, {"Escapães", 9444222},
                    {"Esgueira", 9442028}, {"Esmeriz", 9405033}, {"Esmoriz", 9438406}, {"Espadanal Azamb.", 9431401}, {"Espadaneira", 9435162}, {"Espinho", 9439008}, {"Espinho-Vouga", 9444016}, {"Esqueiro", 9418291}, {"Estarreja", 9438158},
                    {"Estômbar-Lagoa", 9490241}, {"Estoril", 9469245}, {"Estremoz", 9486009}, {"Évora", 9483006}, {"Évora Monte (A)", 9483410}, {"Famal. Da Nazaré", 9463180}, {"Famalicão", 9405074}, {"Fanhais", 9463313}, {"Faria", 9444271},
                    {"Faro", 9473007}, {"Fatela-Penamacor", 9453462}, {"Fátima", 9434249}, {"Fazenda", 9455327}, {"Feliteira", 9462364}, {"Fernando Pó", 9471050}, {"Ferradosa", 9411114}, {"Ferragudo", 9490274}, {"Ferrão", 9410165},
                    {"Ferreiros", 9429132}, {"Figueira Da Foz", 9464113}, {"Figueiredo", 9444412}, {"Fogueteiro", 9417186}, {"Folhadal", 9446656}, {"Fontela", 9464089}, {"Fontela-A", 9464097}, {"Formoselha", 9435030}, {"Fornos Algodres", 9448249},
                    {"Foros De Amora", 9417152}, {"Francelos", 9439081}, {"Fratel", 9452571}, {"Frechas", 9415453}, {"Fregim (A)", 9413110}, {"Fregim-A (A)", 9413102}, {"Freineda", 9449387}, {"Fronteira (A)", 9486355}, {"Frxo. Numão-Mós", 9411247},
                    {"Funcheira", 9477008}, {"Fundão", 9453546}, {"Fungalvaz", 9434199}, {"Fuseta", 9473205}, {"Fuseta-A", 9473197}, {"Gaia-Devesas", 9439164}, {"Gaia-Gen. Torres", 9439172}, {"Garraia (A)", 9483063}, {"Garvão", 9477032},
                    {"Gata", 9449049}, {"Giesteira", 9428159}, {"Godim", 9409423}, {"Gondarém", 9418325}, {"Gouveia", 9448165}, {"Grândola", 9492577}, {"Granja", 9439040}, {"Guarda", 9449007}, {"Guia", 9463800},
                    {"Guifões", 9421170}, {"Guimarães", 9424000}, {"Irivo", 9408243}, {"Jerumelo", 9462257}, {"Juncal", 9409050}, {"Lagos", 9490464}, {"Lamarosa", 9434090}, {"Lapa", 9444057}, {"Lapa Do Lobo", 9446581},
                    {"Lardosa", 9453215}, {"Lares", 9464055}, {"Latadas", 9415495}, {"Lavradio", 9495042}, {"Lavre", 9480515}, {"Leandro", 9404036}, {"Leça Do Balio", 9421071}, {"Leça Do Balio - Petroquímica", 9421709}, {"Leiria", 9463560},
                    {"Leixões", 9421154}, {"Liceia (A)", 9445310}, {"Limede-Cadi. (A)", 9445203}, {"Lisboa-Apolónia", 9430007}, {"Lisboa-Oriente", 9431039}, {"Lisboa-Rossio", 9459006}, {"Litém", 9434504}, {"Livração", 9408474}, {"Livramento", 9473239},
                    {"Lobases", 9441194}, {"Lordelo", 9428191}, {"Loulé", 9478238}, {"Louriçal", 9463875}, {"Louro", 9405116}, {"Lousã", 9441319}, {"Lousã-A", 9441301}, {"Lousado", 9405009}, {"Lousal", 9492809},
                    {"Luso-Buçaco", 9446094}, {"Luz", 9473262}, {"Luzianes", 9477263}, {"Maçainhas", 9454262}, {"Maçal Do Chão", 9448488}, {"Macinhata", 9442325}, {"Madalena", 9439123}, {"Mafra", 9462166}, {"Maiorca (A)", 9445443},
                    {"Mala (A)", 9445047}, {"Malhada Sorda", 9449338}, {"Malveira", 9462224}, {"Mangualde", 9448009}, {"Marco Canaveses", 9409001}, {"Marinha Grande", 9463461}, {"Marinhais", 9480127}, {"Martingança", 9463404}, {"Marujal", 9465052},
                    {"Marvão-Beirã", 9458651}, {"Marvila", 9466019}, {"Massamá-Barcaren", 9460137}, {"Mata", 9456176}, {"Mato De Miranda", 9432383}, {"Mazagão", 9429116}, {"Mealhada", 9437051}, {"Meia Praia", 9490431}, {"Meinedo", 9408359},
                    {"Meiral", 9441293}, {"Meleças", 9462042}, {"Mercês", 9461051}, {"Messines-Alte", 9477735}, {"Mexilhoe. Grande", 9490381}, {"Midões", 9406080}, {"Miramar", 9439073}, {"Miranda Do Corvo", 9441228}, {"Mirandela", 9416006},
                    {"Mirão", 9409225}, {"Miuzela", 9449239}, {"Mogofores", 9437143}, {"Moimenta-Alcaf.", 9446763}, {"Moinhos", 9441186}, {"Moita", 9495109}, {"Moledo Do Minho", 9418200}, {"Monte Abraão", 9460111}, {"Monte Das Flores", 9482214},
                    {"Monte De Lobos", 9446219}, {"Monte De Paramos", 9444040}, {"Monte Estoril", 9469252}, {"Monte Gordo", 9473544}, {"Monte Novo-Palma", 9492205}, {"Monte Real", 9463685}, {"Monte Redondo", 9463735}, {"Montemor", 9465029}, {"Morgado", 9480044},
                    {"Mortágua", 9446243}, {"Moscavide", 9431047}, {"Mosteirô", 9409134}, {"Mouquim", 9405108}, {"Mourisca Vouga", 9442259}, {"Mouriscas-A", 9452167}, {"Mourisca-Sado", 9491041}, {"Muge", 9480077}, {"Murtede (A)", 9445104},
                    {"Nelas", 9446672}, {"Nespereira", 9428266}, {"Nine", 9406007}, {"Noemi", 9449270}, {"Óbidos", 9462836}, {"Oeiras", 9469179}, {"Oiã", 9437275}, {"Oleiros", 9408250}, {"Olhão", 9473106},
                    {"Oliv. Azeméis", 9444339}, {"Oliveira", 9408409}, {"Oliveira Bairro", 9437218}, {"Oliveirinha-Cab.", 9446524}, {"Oronhe", 9442176}, {"Ourique", 9476158}, {"Outeiro", 9462612}, {"Ovar", 9438299}, {"Paço De Arcos", 9469146},
                    {"Paços De Brandão", 9444107}, {"Padrão", 9441251}, {"Paialvo", 9434157}, {"Pala", 9409100}, {"Palmela-A", 9468098}, {"Palmilheira", 9403053}, {"Pampilhosa", 9437002}, {"Panóias", 9476216}, {"Papízios", 9446441},
                    {"Parada", 9408201}, {"Paraimo", 9437184}, {"Paramos", 9438414}, {"Parede", 9469203}, {"Paredes", 9408276}, {"Parque Cidades", 9478295}, {"Passinhos (A)", 9413086}, {"Patã", 9478105}, {"Pataias", 9463354},
                    {"Paul", 9462745}, {"Pedra Furada", 9462133}, {"Pego", 9446060}, {"Pegões", 9471126}, {"Pelariga", 9434702}, {"Penafiel", 9408318}, {"Penalva", 9417095}, {"Penedo Gordo", 9475085}, {"Penteado", 9495125},
                    {"Pereira", 9435055}, {"Pereiras", 9477461}, {"Pereirinhas", 9428217}, {"Pero Negro", 9462315}, {"Pin. Da Bemposta", 9444446}, {"Pinhal Novo", 9468007}, {"Pinhão", 9410249}, {"Pinheiro", 9492130}, {"Pinhel", 9448595},
                    {"Plataf.Cacia", 9438034}, {"Poceirão", 9471001}, {"Pocinho", 9412005}, {"Poço Barreto", 9490134}, {"Pombal", 9434645}, {"Ponte De Sôr", 9455293}, {"Porta Nova", 9473338}, {"Portalegre", 9457000}, {"Portela", 9404101},
                    {"Portela Sintra", 9461093}, {"Portimão", 9490290}, {"Porto De Rei", 9409282}, {"Porto-Campanhã", 9402006}, {"Porto-São Bento", 9401008}, {"Póvoa", 9431146}, {"Povoação", 9414126}, {"Praça Do Quebedo", 9468130}, {"Pragal", 9417087},
                    {"Praia Ribatejo", 9451128}, {"Praias-Sado", 9491009}, {"Praias-Sado-A", 9491058}, {"Prilhão-Casais", 9441350}, {"Queluz-Belas", 9460103}, {"Quinta Da Ponte", 9441087}, {"Quinta Grande", 9480358}, {"Quintans", 9437358}, {"Ramalhal", 9462547},
                    {"Reboleira", 9460095}, {"Recarei-Sobreira", 9408177}, {"Recesinhos", 9408441}, {"Rede", 9409357}, {"Régua", 9410009}, {"Regueira Pontes", 9463610}, {"Reguengo", 9433084}, {"Retaxo", 9452845}, {"Reveles", 9465128},
                    {"Riachos", 9432466}, {"Rib. Do Freixo (A)", 9486587}, {"Ribeira De Seiça", 9463925}, {"Ribeirinha (A)", 9415347}, {"Rio De Mouro", 9461044}, {"Rio Meão", 9444115}, {"Rio Tinto", 9403038}, {"Rochoso", 9449163}, {"Ródão", 9452647},
                    {"Roma-Areeiro", 9466035}, {"Ruílhe", 9429074}, {"Runa", 9462422}, {"S. J. Craveiras", 9471159}, {"S. João Estoril", 9469237}, {"S. M. Do Porto", 9463131}, {"S. Pedro Estoril", 9469229}, {"S.Bart.Serra", 9494227}, {"S.Cruz-Damaia", 9460038},
                    {"S.J. Da Madeira", 9444255}, {"S.Mamede Infesta", 9421048}, {"S.Mart. Do Campo", 9408102}, {"S.Tiago R-Ul", 9444313}, {"Sabugal", 9454429}, {"Sabugo", 9462091}, {"Sacavém", 9431062}, {"Salgueirinha", 9480408}, {"Salir Do Porto", 9463107},
                    {"Salreu", 9438125}, {"Sampaio-Oleiros", 9444073}, {"Sanfins", 9444198}, {"Santa Cita", 9440105}, {"Santa Comba-Dão", 9446367}, {"Santa Eulália-A", 9457315}, {"Santa Iria", 9431112}, {"Santa Luzia", 9415149}, {"Santa Margarida", 9451185},
                    {"Santa Vitória-Ervidel", 9475176}, {"Santana-Cartaxo", 9432045}, {"Santana-Fer. (Ad)", 9445351}, {"Santarém", 9432185}, {"Santo Amaro", 9469161}, {"Santo Tirso", 9428068}, {"Santos", 9469013}, {"São Frutuoso", 9404051}, {"São Gemil", 9421006},
                    {"São João De Ver", 9444156}, {"São João Loure", 9442093}, {"São José", 9441053}, {"São Lourenço (A)", 9415164}, {"São Mamede", 9462786}, {"São Mamede Tua", 9410363}, {"São Marcos", 9477594}, {"São Pedro Torre", 9418440}, {"São Romão", 9404077},
                    {"São Torcato", 9480465}, {"Sapataria", 9462299}, {"Sarnadas", 9452803}, {"Seiça-Ourém", 9434264}, {"Seixas", 9418275}, {"Sernada Do Vouga", 9443000}, {"Serpins", 9441376}, {"Sete Rios", 9466076}, {"Setil", 9432003},
                    {"Setúbal", 9468122}, {"Setúbal-Mar", 9468155}, {"Silva", 9406155}, {"Silvã-Feitei (A)", 9445062}, {"Silvalde", 9438422}, {"Silvalde-Vouga", 9444032}, {"Silveirona (A)", 9486074}, {"Silves", 9490183}, {"Simões", 9434744},
                    {"Sintra", 9461101}, {"Soalheira", 9453264}, {"Sobral", 9448728}, {"Soito", 9446185}, {"Somincor", 9492478}, {"Soudos-Vila Nova", 9440014}, {"Soure", 9434801}, {"Sousa Da Sé (A)", 9483113}, {"Sousel", 9486199},
                    {"Souselas", 9436087}, {"Sra. Da Agonia", 9418234}, {"Sra. Das Neves", 9406312}, {"Sra.Dores (Inactivo)", 9404135}, {"Sta Clara-Sabóia", 9477388}, {"Sto.Amaro-Veiros(A)", 9486306}, {"Sto.Isidoro (A)", 9413037}, {"Suzão", 9408060}, {"T.M.Bobadela", 9431120},
                    {"T.Mer.Fundão", 9453579}, {"T.Portaveiro", 9438604}, {"Tadim", 9429090}, {"Taipa-Requeixo", 9442127}, {"Tamel", 9406213}, {"Tancos", 9451086}, {"Tanha (A)", 9414043}, {"Tarana", 9416014}, {"Taveiro", 9435139},
                    {"Tavira", 9473320}, {"Telhada", 9463958}, {"Telhal", 9462067}, {"Terronhas", 9408136}, {"Ter-Tir", 9431807}, {"Tojeirinha", 9452738}, {"Tomar", 9440154}, {"Torre Da Gadanha", 9472199}, {"Torres Vedras", 9462471},
                    {"Tortosendo", 9453678}, {"Tralhão (A)", 9415180}, {"Tralhariz (A)", 9415057}, {"Tramagal", 9451243}, {"Trancoso", 9408151}, {"Travagem", 9404010}, {"Travanca-Macin.", 9444396}, {"Travassô", 9442135}, {"Tre. Das Vargens", 9456002},
                    {"Trémoa", 9441152}, {"Trezói", 9446136}, {"Tri.Sid.Nac.", 9481034}, {"Trofa", 9404630}, {"Tua", 9411007}, {"Tunes", 9478006}, {"Ul", 9444362}, {"Urgueiras", 9444537}, {"V. F. Das Naves", 9448546},
                    {"V. N. De Anços", 9434868}, {"V. Nv. Da Rainha", 9431377}, {"V. P. Do Campo", 9435105}, {"V.N. Da Baronia", 9474278}, {"V.N. De Cerveira", 9418341}, {"V.R.Sto.António", 9473569}, {"Vacariça", 9446045}, {"Valadares", 9439115}, {"Valado", 9463263},
                    {"Valbom (A)", 9413052}, {"Vale Da Rosa", 9491033}, {"Vale De Açor", 9441129}, {"Vale De Figueira", 9432284}, {"Vale De Guizo", 9492437}, {"Vale De Judeu", 9478196}, {"Vale De Prazeres", 9453397}, {"Vale De Santarém", 9432102}, {"Vale Do Peso", 9458305},
                    {"Vale Pereiro (A)", 9483253}, {"Vale Peso-A", 9458271}, {"Válega", 9438240}, {"Valença", 9407005}, {"Valongo", 9408086}, {"Valongo Vouga", 9442291}, {"Vargelas", 9411148}, {"Venda Do Alcaide", 9468049}, {"Vendas Novas", 9472009},
                    {"Vermoil", 9434553}, {"Verride", 9465086}, {"Vesúvio", 9411197}, {"Viana", 9474203}, {"Viana Castelo", 9418002}, {"Vidigal", 9480655}, {"Vila Caiz", 9413078}, {"Vila Da Feira", 9444206}, {"Vila Das Aves", 9428134},
                    {"Vila Fernando", 9449114}, {"Vila Fr. De Xira", 9431278}, {"Vila Garcia", 9449072}, {"Vila Meã", 9408433}, {"Vila Real", 9414266}, {"Vilar Formoso", 9449460}, {"Vilarinho (A)", 9415388}, {"Vilela-Fornos", 9436053}, {"Vimieiro", 9483337},
                    {"Virtudes", 9433043}, {"Vizela", 9428233}, {"Zibreira", 9462331}
            };
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }
    }

    public class NodesComboioTabelsPartidasChegada
    {
        public bool ComboioPassou { get; set; }
        public string? DataHoraPartidaChegada { get; set; }
        public string? DataHoraPartidaChegada_ToOrderBy { get; set; }
        public DateTime DataHoraPartidaChegada_ToOrderByi { get; set; }
        public int EstacaoDestino { get; set; }
        public int EstacaoOrigem { get; set; }
        public int NComboio1 { get; set; }
        public int NComboio2 { get; set; }
        public string? NomeEstacaoDestino { get; set; }
        public string? NomeEstacaoOrigem { get; set; }
        public string? Observacoes { get; set; }
        public string? Operador { get; set; }
        public string? TipoServico { get; set; }
    }

    public class Response
    {
        public int NodeID { get; set; }
        public List<NodesComboioTabelsPartidasChegada> NodesComboioTabelsPartidasChegadas { get; set; }
        public string NomeEstacao { get; set; }
        public int TipoPedido { get; set; }
    }

    public class Root
    {
        public List<Response>? Response { get; set; }
    }
}
