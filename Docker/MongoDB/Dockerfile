# syntax=docker/dockerfile:1
FROM mongo

WORKDIR /app

# Setup MongoDB as single-node replicaset
RUN mkdir -p /data/db /data/configdb \
    && chown -R mongodb:mongodb /data/db /data/configdb \
    && mongod --logpath /var/log/mongodb/initdb.log --replSet "rs0" --bind_ip 0.0.0.0 --fork \
    && mongo --eval 'rs.initiate({_id: "rs0", members: [{ _id: 0, host: "localhost:27017"}]})' \
    && mongod --shutdown

COPY ./entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT [ "/app/entrypoint.sh" ]