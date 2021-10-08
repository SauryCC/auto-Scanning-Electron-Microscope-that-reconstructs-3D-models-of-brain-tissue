namespace MIcrotome_GUI
{
    public class Coordinate
    {
        public Coordinate(uint ID, double x, double y)
        {
            // x y in um
            this.id = ID;
            this.x = x;
            this.y = y;
            this.width = CNaviManage.SEM_initial_length / 6.0;  // in um
            this.height = CNaviManage.SEM_initial_length / (double)CNaviManage.SEM_x_pixel * CNaviManage.SEM_y_pixel / 6;
            this.magification = 6;  // minimum value
            this.focus = -1;
        }

        public Coordinate(uint ID, double x, double y, int mag)
        {
            this.id = ID;
            this.x = x;
            this.y = y;
            this.magification = mag;
            this.width = CNaviManage.SEM_initial_length / (double)magification;  // in um
            this.height = CNaviManage.SEM_initial_length / (double)CNaviManage.SEM_x_pixel * CNaviManage.SEM_y_pixel / magification;  // in um
            this.focus = -1;
        }

        public Coordinate(uint ID, double x, double y, int mag, int focus)
        {
            this.id = ID;
            this.x = x;
            this.y = y;
            this.magification = mag;
            this.width = CNaviManage.SEM_initial_length / (double)magification;  // in um
            this.height = CNaviManage.SEM_initial_length / (double)CNaviManage.SEM_x_pixel * CNaviManage.SEM_y_pixel / magification;  // in um
            this.focus = focus;
        }

        public uint id { get; set; }

        public double x { get; set; }

        public double y { get; set; }

        public double width { get; set; }

        public double height { get; set; }

        public int magification { get; set; }

        public int focus { get; set; }

        public string name
        {
            get { return string.Format("ROI(id: {0}, {1:F2}, {2:F2}), f:{3}, m:{4}", id, x, y, focus, magification); }
        }

    }
}
