namespace Eticadata.Cust.DeskTop.Views
{
    //É necessário ter a referencia da dll em que se encontra na base, neste caso é Eticadata.Views.Tables.Entities
    public partial class usrCustomers : Eticadata.Views.Tables.Entities.ClientesView
    {
        public usrCustomers()
        {
            InitializeComponent();

            //Mudar o titulo de forma a saber que foi substituida
            this.Tag = "FORMAÇÂO: " + this.Tag;
        }

        /// <summary>
        ///É possivel colocar código em todos os métodos override
        ///Quando se posiciona num registo passa sempre por este método
        /// </summary>
        protected override void Depois_Posicionar()
        {
            base.Depois_Posicionar();
        }

        /// <summary>
        /// Depois de gravar o registo
        /// </summary>
        protected override void Depois_Gravar()
        {
            base.Depois_Gravar();
        }

        /// <summary>
        /// Antes de gravar o registo
        /// </summary>
        /// <returns></returns>
        protected override bool Antes_Gravar()
        {
            return base.Antes_Gravar();
        }
    }
}
