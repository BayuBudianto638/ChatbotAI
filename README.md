Ini adalah aplikasi chatbot percobaan menggunakan GeminiAI.
Kenapa ke Gemini AI? Krn gratis dan murah. Sebelumnya menggunakan LLamaSharp ternyata generate textnya memakan waktu krn lappie tidak memiliki CUDA dan hanya quad core.

ChatBot UI untuk Kacrut AI

FE menggunakan ASP.NET MVC (Razor Pages + JQuery)

Untuk BE menggunakan .NET CORE 8, MongoDB untuk Database dan Weaviate untuk Vectorize Database sebagai pusat knowledge.
Disini saya menggunakan Docker untuk Weaviate dan Mongo nya.

Script YML terlampir.

Milestone kedepan:
Membuat custom query untuk Weaviate agar pencarian di Weaviate lebih cepat dan akurat untuk di baca oleh AI nya

Docker untuk weaviate

services:
  weaviate:
    image: semitechnologies/weaviate:latest
    restart: always
    ports:
      - "8080:8080"
    environment:
      QUERY_DEFAULTS_LIMIT: 100
      AUTHENTICATION_ANONYMOUS_ACCESS_ENABLED: "true"
      PERSISTENCE_DATA_PATH: "/var/lib/weaviate"
      ENABLE_MODULES: "text2vec-transformers"
      TRANSFORMERS_INFERENCE_API: "http://t2v-transformers:8080"
      READ_ONLY: "false"
    volumes:
      - weaviate_data:/var/lib/weaviate

  t2v-transformers:
    image: semitechnologies/transformers-inference:sentence-transformers-all-MiniLM-L6-v2
    restart: always

volumes:
  weaviate_data:


Docker untuk MongoDB

services:
  mongodb:
    image: mongo:latest
    container_name: mongodb
    restart: always
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: adminpassword
    ports:
      - "27017:27018"  
    volumes:
      - mongo_data:/data/db

volumes:
  mongo_data:

  ![image](https://github.com/user-attachments/assets/b4ed2a0b-5255-4f86-bf56-33bb1eda921e)


  ![image](https://github.com/user-attachments/assets/ce28afcb-eaba-4669-8b10-e5409e9f016d)



