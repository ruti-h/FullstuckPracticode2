import axios from 'axios';


axios.defaults.baseURL=API.env.REACT_APP_API_KEY 
axios.interceptors.response.use(
  response => response, 
  error => {
    console.error('Axios Error:', error.response ? error.response.data : error.message);
    //return Promise.reject(error); // דחוף את השגיאה כדי שתוכל לטפל בה במקום אחר
  }
);

export default {
  getTasks: async () => {
    try {
      const result = await axios.get(`${apiUrl}/items`);
      return result.data;
    } catch (error) {
      console.error("Error fetching tasks:", error);
      return [];
    }
  },

  addTask: async (name) => {
    try {
      const newTask = { name, isComplete: false };
      const result = await axios.post(`${apiUrl}/items`, newTask);
      return result.data;
    } catch (error) {
      console.error("Error adding task:", error);
      return null;
    }
  },

  setCompleted: async (id, isComplete) => {
    try {
      const updatedTask = { isComplete };
      await axios.put(`${apiUrl}/items/${id}`, updatedTask);
      return { success: true };
    } catch (error) {
      console.error("Error updating task:", error);
      return { success: false };
    }
  },
  // setCompleted: async (id, isComplete) => {
  //   try {
  //     // שים לב: כאן אנחנו לא שולחים את isComplete, אלא רק מבקשים מהשרת לבצע toggle
  //     const response = await axios.put(`${apiUrl}/items/${id}`, {}); 
  //     // אם תרצי לקבל את האובייקט המעודכן, response.data יכיל את המצב החדש
  //     return { success: true, updatedItem: response.data };
  //   } catch (error) {
  //     console.error("Error updating task:", error);
  //     return { success: false };
  //   }
  // },

  deleteTask: async (id) => {
    try {
      await axios.delete(`${apiUrl}/items/${id}`);
      return { success: true };
    } catch (error) {
      console.error("Error deleting task:", error);
      return { success: false };
    }
  },
};
