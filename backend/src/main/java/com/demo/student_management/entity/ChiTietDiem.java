package com.demo.student_management.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.*;

import java.math.BigDecimal;

@Entity
@Table(name = "CHITIET_DIEM")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class ChiTietDiem {

    @EmbeddedId
    @EqualsAndHashCode.Include
    private ChiTietDiemId id;

    @MapsId("idHocSinh")
    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_HOCSINH", nullable = false)
    private HocSinh hocSinh;

    @MapsId("idMonHoc")
    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_MONHOC", nullable = false)
    private MonHoc monHoc;

    @MapsId("idHocKy")
    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_HOCKY", nullable = false)
    private HocKy hocKy;

    @Column(name = "DIEM15", precision = 4, scale = 2)
    private BigDecimal diem15;

    @Column(name = "DIEM45", precision = 4, scale = 2)
    private BigDecimal diem45;

    @Column(name = "DIEMTB", precision = 4, scale = 2)
    private BigDecimal diemTb;
}